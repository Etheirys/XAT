#include "FbxToHavok.hpp"
#include "../Utils/Math.hpp"
#include "../FBX/FbxUtils.hpp"
#include "../FatalException.hpp"
#include <iostream>
#include <vector>
#include <string>
#include <algorithm>
#include <map>

int FbxToHavok::ConvertSkeleton(FbxScene* sourceScene, hkaSkeleton* targetSkeleton, const std::vector<std::string>& boneOrder)
{
	std::cout << "Converting skeleton..." << std::endl;

	// Get just bones
	FbxBoneMap boneMap = FbxBoneMap::Create(sourceScene);
	int validCount = boneMap.Size();
	std::cout << "Found " << validCount << " bones to convert." << std::endl;

	std::vector<std::string> convertOrder;

	for (const std::string& boneName : boneOrder)
	{
		FbxNode* node = boneMap.GetNodeByName(boneName.c_str());
		if (!node)
			throw FatalException("Could not find bone " + boneName + ". All ordered bones must be present to maintain compatibility.");

		convertOrder.push_back(boneName);
	}

	for (int i = 0; i < validCount; ++i)
	{
		FbxNode* node = boneMap.GetNodeAtIndex(i);
		std::string boneName = node->GetName();
		bool alreadyDone = std::find(convertOrder.begin(), convertOrder.end(), boneName) != convertOrder.end();
		if (!alreadyDone)
		{
			convertOrder.push_back(boneName);
		}
	}

	if (convertOrder.size() != validCount)
		throw FatalException("Incorrect number of bones");

	// Create the havok structure
	for (int i = 0; i < validCount; ++i)
	{
		const std::string& boneName = convertOrder[i];
		FbxNode* node = boneMap.GetNodeByName(boneName.c_str());

		targetSkeleton->m_bones.expandOne();
		targetSkeleton->m_parentIndices.expandOne();
		targetSkeleton->m_referencePose.expandOne();

		hkStringBuf nameBuf(boneName.c_str(), boneName.length());
		targetSkeleton->m_bones[i].m_name = nameBuf;
		targetSkeleton->m_bones[i].m_lockTranslation = false;

		// Find valid parent
		int parentIndex = -1;
		FbxNode* parent = node->GetParent();
		if (parent)
		{
			if (sourceScene->GetRootNode() != parent) // Skip root
			{

				const std::string& parentBoneName = parent->GetName();
				auto parentIt = std::find(convertOrder.begin(), convertOrder.end(), parentBoneName);

				if (parentIt == convertOrder.end())
					throw FatalException("Could not find parent (" + parentBoneName + ") for bone (" + boneName + ").");

				parentIndex = std::distance(convertOrder.begin(), parentIt);
			}
		}
		targetSkeleton->m_parentIndices[i] = parentIndex;

		// Set reference pose
		targetSkeleton->m_referencePose[i].m_rotation = Vector4::From(node->LclRotation.Get()).ToQuat().ToHkQuaternion();
		targetSkeleton->m_referencePose[i].m_translation = Vector4::From(node->LclTranslation.Get()).ToHkVector4();
		targetSkeleton->m_referencePose[i].m_scale = Vector4::From(node->LclScaling.Get()).ToHkVector4();
	}

	std::cout << "Converted skeleton." << std::endl;

	return validCount;
}

int FbxToHavok::ConvertAnimation(FbxScene* sourceScene, FbxAnimStack* sourceAnim, hkaSkeleton* sourceSkeleton, hkaInterleavedUncompressedAnimation* targetAnimation, hkaAnimationBinding* targetBinding, const std::vector<std::string>& excludeBones)
{
	std::cout << "Converting animation..." << std::endl;

	FbxBoneMap sourceBoneMap = FbxBoneMap::Create(sourceScene);

	int numTargetTracks = sourceSkeleton->m_bones.getSize();
	int numSourceTracks = sourceBoneMap.Size();
	FbxTime startTime = sourceAnim->GetReferenceTimeSpan().GetStart();
	FbxTime stopTime = sourceAnim->GetReferenceTimeSpan().GetStop();
	FbxTime::EMode timeMode = FbxTime::eFrames30;
	int startFrame = (int)startTime.GetFrameCount(timeMode);
	int stopFrame = (int)stopTime.GetFrameCount(timeMode);
	int frameCount = stopFrame - startFrame + 1;
	float duration = (float)stopTime.GetSecondDouble();

	std::cout << "Found " << numSourceTracks << " source bones and " << numTargetTracks << " target bones across " << frameCount << " frames (" << duration << "s)." << std::endl;

	// Build bone map
	std::map<int, int> boundMap;
	std::cout << "Binding bones..." << std::endl;
	for (int track = 0; track < numTargetTracks; ++track)
	{
		std::string boneName = std::string(sourceSkeleton->m_bones[track].m_name);

		if (boneName == "n_root") // We never bind the root
			continue;

		bool didFindExclusion = std::find(excludeBones.begin(), excludeBones.end(), boneName) != excludeBones.end();
		if (didFindExclusion)
			continue;

		int nodeId = sourceBoneMap.GetIndexByName(boneName.c_str());
		if (nodeId != -1)
		{
			boundMap[track] = nodeId;
		}
	}
	std::cout << "Finished binding bones, successfully bound " << boundMap.size() << " (out of " << numSourceTracks << " requested and " << numTargetTracks << " possible)." << std::endl;

	int validTrackCount = boundMap.size();

	// Anim Setup
	targetAnimation->m_duration = duration;
	targetAnimation->m_numberOfTransformTracks = validTrackCount;
	targetAnimation->m_transforms.setSize(validTrackCount * frameCount, hkQsTransform::getIdentity());
	targetAnimation->m_annotationTracks.clear();
	targetAnimation->m_annotationTracks.setSize(0);
	targetAnimation->m_numberOfFloatTracks = 0;
	targetAnimation->m_floats.setSize(0);

	// Binding Setup
	targetBinding->m_animation = targetAnimation;
	targetBinding->m_originalSkeletonName = sourceSkeleton->m_name;
	targetBinding->m_transformTrackToBoneIndices.setSize(validTrackCount);

	// Frame conversion
	std::cout << "Converting " << frameCount << " frames..." << std::endl;
	FbxAnimEvaluator* animEvaluator = sourceScene->GetAnimationEvaluator();
	int currentTrack = 0;
	FbxTime currentFbxTime(0);
	currentFbxTime.SetGlobalTimeMode(timeMode);
	for (auto trackInfo = boundMap.begin(); trackInfo != boundMap.end(); ++trackInfo, ++currentTrack)
	{
		targetBinding->m_transformTrackToBoneIndices[currentTrack] = trackInfo->first;
		FbxNode* sourceBone = sourceBoneMap.GetNodeAtIndex(trackInfo->second);
		std::string sourceBoneName = std::string(sourceBone->GetName());

		for (int frame = 0; frame < frameCount; ++frame)
		{
			currentFbxTime.SetFrame(frame);

			FbxAMatrix sourceBoneTransform = animEvaluator->GetNodeLocalTransform(sourceBone, currentFbxTime);

			hkVector4 t = Vector4::From(sourceBoneTransform.GetT()).ToHkVector4();
			hkQuaternion r = Vector4::From(sourceBoneTransform.GetQ()).ToHkQuaternion();
			hkVector4 s = Vector4::From(sourceBoneTransform.GetS()).ToHkVector4();

			targetAnimation->m_transforms[frame * validTrackCount + currentTrack].set(t, r, s);
		}
	}
	std::cout << "Converted " << frameCount << " frames." << std::endl;

	hkaSkeletonUtils::normalizeRotations(targetAnimation->m_transforms.begin(), targetAnimation->m_transforms.getSize());

	std::cout << "Converted animation." << std::endl;

	return frameCount;
}