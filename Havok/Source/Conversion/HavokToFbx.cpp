#include "HavokToFbx.hpp"

#include <iostream>

#include "../FBX/FbxUtils.hpp"
#include "../Utils/Math.hpp"

int HavokToFbx::ConvertSkeleton(hkaSkeleton* sourceSkeleton, FbxScene* targetScene)
{
	std::cout << "Converting skeleton..." << std::endl;

	int boneCount = sourceSkeleton->m_bones.getSize();

	std::cout << "Building " << boneCount << " bones...." << std::endl;
	for (int boneIdx = 0; boneIdx < boneCount; ++boneIdx)
	{
		const hkaBone& bone = sourceSkeleton->m_bones[boneIdx];

		hkQsTransform hkTransform = sourceSkeleton->m_referencePose[boneIdx];
		Vector4 t = Vector4::From(hkTransform.getTranslation());
		Vector4 r = Vector4::From(hkTransform.getRotation());
		Vector4 s = Vector4::From(hkTransform.getScale());

		FbxSkeleton* skeletonProperty = FbxSkeleton::Create(targetScene, "XATSkeleton");

		if (boneIdx == 0)
		{
			skeletonProperty->SetSkeletonType(FbxSkeleton::eRoot);
		}
		else
		{
			skeletonProperty->SetSkeletonType(FbxSkeleton::eLimbNode);
		}

		FbxNode* joint = FbxNode::Create(targetScene, bone.m_name);
		joint->SetNodeAttribute(skeletonProperty);
		joint->LclTranslation.Set(t.ToFbxVector4());
		joint->LclRotation.Set(r.ToEuler().ToFbxVector4());
		joint->LclScaling.Set(s.ToFbxVector4());

		targetScene->GetRootNode()->AddChild(joint);
	}
	std::cout << "Bones built." << std::endl;

	std::cout << "Parenting bones..." << std::endl;
	FbxBoneMap boneMap = FbxBoneMap::Create(targetScene);
	for (int boneIdx = 0; boneIdx < boneCount; ++boneIdx)
	{
		int parentIdx = sourceSkeleton->m_parentIndices[boneIdx];

		if (parentIdx == -1)
			continue;

		const char* parentBoneName = sourceSkeleton->m_bones[parentIdx].m_name;
		const char* currentBoneName = sourceSkeleton->m_bones[boneIdx].m_name;

		FbxNode* parentNode = boneMap.GetNodeByName(parentBoneName);
		FbxNode* childNode = boneMap.GetNodeByName(currentBoneName);

		parentNode->AddChild(childNode);
	}

	std::cout << "Bones parented." << std::endl;

	std::cout << "Skeleton converted." << std::endl;

	return boneCount;
}

int HavokToFbx::ConvertAnimation(hkaAnimation* sourceAnimation, hkaAnimationBinding* sourceBinding, hkaSkeleton* sourceSkeleton, FbxScene* targetScene)
{
	std::cout << "Converting animation..." << std::endl;
	FbxBoneMap fbxBoneMap = FbxBoneMap::Create(targetScene);

	FbxAnimStack* animStack = FbxAnimStack::Create(targetScene, "xat");
	FbxAnimLayer* animLayer = FbxAnimLayer::Create(targetScene, "0");
	animStack->AddMember(animLayer);

	int numBones = sourceSkeleton->m_bones.getSize();
	int numTracks = numBones;

	float duration = (float)sourceAnimation->m_duration;
	int frameCount = sourceAnimation->getNumOriginalFrames();
	float frameTime = (float)duration / (frameCount - 1);

	std::cout << "Found " << numTracks << " bones across " << frameCount << " frames (" << duration << "s)." << std::endl;

	hkaAnimatedSkeleton* animatedSkeleton = new hkaAnimatedSkeleton(sourceSkeleton);
	hkaDefaultAnimationControl* control = new hkaDefaultAnimationControl(sourceBinding);
	animatedSkeleton->addAnimationControl(control);
	hkaPose pose(animatedSkeleton->getSkeleton());

	std::cout << "Converting " << frameCount << " frames..." << std::endl;
	float realTime = 0.0f;
	FbxTime fbxTime;
	for (int frame = 0; frame < frameCount; ++frame, realTime += frameTime)
	{
		bool canCreate = frame == 0;
		fbxTime.SetTime(0, 0, 0, frame, 0, 0, fbxTime.eFrames30);
		control->setLocalTime(realTime);

		animatedSkeleton->sampleAndCombineAnimations(pose.accessUnsyncedPoseModelSpace().begin(), pose.getFloatSlotValues().begin());
		const hkQsTransform* transforms = pose.getSyncedPoseModelSpace().begin();

		for (int trackIdx = 0; trackIdx < numTracks; ++trackIdx)
		{
			hkaBone& bone = sourceSkeleton->m_bones[trackIdx];
			FbxNode* node = fbxBoneMap.GetNodeByName(bone.m_name);

			if (!node)
				continue;
			
			hkQsTransform transform = transforms[trackIdx];
			Vector4 t = Vector4::From(transform.getTranslation());
			Vector4 r = Vector4::From(transform.getRotation()).ToEuler();
			Vector4 s = Vector4::From(transform.getScale());
			
			int keyIndex = 0;

			// Translation
			FbxAnimCurve* tCurveX = node->LclTranslation.GetCurve(animLayer, FBXSDK_CURVENODE_COMPONENT_X, canCreate);
			tCurveX->KeyModifyBegin();
			keyIndex = tCurveX->KeyAdd(fbxTime);
			tCurveX->KeySetValue(keyIndex, t.x);
			tCurveX->KeySetInterpolation(keyIndex, FbxAnimCurveDef::eInterpolationConstant);
			tCurveX->KeyModifyEnd();

			FbxAnimCurve* tCurveY = node->LclTranslation.GetCurve(animLayer, FBXSDK_CURVENODE_COMPONENT_Y, canCreate);
			tCurveY->KeyModifyBegin();
			keyIndex = tCurveY->KeyAdd(fbxTime);
			tCurveY->KeySetValue(keyIndex, t.y);
			tCurveY->KeySetInterpolation(keyIndex, FbxAnimCurveDef::eInterpolationConstant);
			tCurveY->KeyModifyEnd();

			FbxAnimCurve* tCurveZ = node->LclTranslation.GetCurve(animLayer, FBXSDK_CURVENODE_COMPONENT_Z, canCreate);
			tCurveZ->KeyModifyBegin();
			keyIndex = tCurveZ->KeyAdd(fbxTime);
			tCurveZ->KeySetValue(keyIndex, t.z);
			tCurveZ->KeySetInterpolation(keyIndex, FbxAnimCurveDef::eInterpolationConstant);
			tCurveZ->KeyModifyEnd();

			// Rotation
			FbxAnimCurve* rCurveX = node->LclRotation.GetCurve(animLayer, FBXSDK_CURVENODE_COMPONENT_X, canCreate);
			rCurveX->KeyModifyBegin();
			keyIndex = rCurveX->KeyAdd(fbxTime);
			rCurveX->KeySetValue(keyIndex, r.x);
			rCurveX->KeySetInterpolation(keyIndex, FbxAnimCurveDef::eInterpolationConstant);
			rCurveX->KeyModifyEnd();

			FbxAnimCurve* rCurveY = node->LclRotation.GetCurve(animLayer, FBXSDK_CURVENODE_COMPONENT_Y, canCreate);
			rCurveY->KeyModifyBegin();
			keyIndex = rCurveY->KeyAdd(fbxTime);
			rCurveY->KeySetValue(keyIndex, r.y);
			rCurveY->KeySetInterpolation(keyIndex, FbxAnimCurveDef::eInterpolationConstant);
			rCurveY->KeyModifyEnd();

			FbxAnimCurve* rCurveZ = node->LclRotation.GetCurve(animLayer, FBXSDK_CURVENODE_COMPONENT_Z, canCreate);
			rCurveZ->KeyModifyBegin();
			keyIndex = rCurveZ->KeyAdd(fbxTime);
			rCurveZ->KeySetValue(keyIndex, r.z);
			rCurveZ->KeySetInterpolation(keyIndex, FbxAnimCurveDef::eInterpolationConstant);
			rCurveZ->KeyModifyEnd();

			// Scale
			FbxAnimCurve* sCurveX = node->LclScaling.GetCurve(animLayer, FBXSDK_CURVENODE_COMPONENT_X, canCreate);
			sCurveX->KeyModifyBegin();
			keyIndex = sCurveX->KeyAdd(fbxTime);
			sCurveX->KeySetValue(keyIndex, s.x);
			sCurveX->KeySetInterpolation(keyIndex, FbxAnimCurveDef::eInterpolationConstant);
			sCurveX->KeyModifyEnd();

			FbxAnimCurve* sCurveY = node->LclScaling.GetCurve(animLayer, FBXSDK_CURVENODE_COMPONENT_Y, canCreate);
			sCurveY->KeyModifyBegin();
			keyIndex = sCurveY->KeyAdd(fbxTime);
			sCurveY->KeySetValue(keyIndex, s.y);
			sCurveY->KeySetInterpolation(keyIndex, FbxAnimCurveDef::eInterpolationConstant);
			sCurveY->KeyModifyEnd();

			FbxAnimCurve* sCurveZ = node->LclScaling.GetCurve(animLayer, FBXSDK_CURVENODE_COMPONENT_Z, canCreate);
			sCurveZ->KeyModifyBegin();
			keyIndex = sCurveZ->KeyAdd(fbxTime);
			sCurveZ->KeySetValue(keyIndex, s.z);
			sCurveZ->KeySetInterpolation(keyIndex, FbxAnimCurveDef::eInterpolationConstant);
			sCurveZ->KeyModifyEnd();
		}
	}
	std::cout << "Converted " << frameCount << " frames." << std::endl;

	std::cout << "Converted animation." << std::endl;

	delete animatedSkeleton;
	delete control;

	return frameCount;
}
