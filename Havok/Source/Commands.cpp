#include <iostream>
#include <algorithm>

#include "Commands.hpp"

#include "FatalException.hpp"

#include "Havok/HavokHeaders.hpp"
#include "Havok/HavokSystem.hpp"
#include "Havok/HavokUtils.hpp"

#include "Conversion/FbxToHavok.hpp"
#include "Conversion/HavokToFbx.hpp"

#include "FBX/FbxSystem.hpp"
#include "FBX/FbxUtils.hpp"

#include "Utils/IOUtils.hpp"

#define ITEM_DELIM "/"

void Commands::Run(const std::string& command, const std::vector<std::string>& arguments)
{
	HavokSystem::Instance().Init();
	FbxSystem::Instance().Init();

	if (command == "getStats")
	{
		Commands::GetStats(arguments[0]);
	}

	else if (command == "createContainer")
	{
		Commands::CreateContainer(arguments[0]);
	}

	else if (command == "addAnimation")
	{
		Commands::AddAnimation(arguments[0], arguments[1], std::stoi(arguments[2]), arguments[3]);
	}

	else if (command == "replaceAnimation")
	{
		Commands::ReplaceAnimation(arguments[0], std::stoi(arguments[1]), arguments[2], std::stoi(arguments[3]), arguments[4]);
	}

	else if (command == "removeAnimation")
	{
		Commands::RemoveAnimation(arguments[0], std::stoi(arguments[1]), arguments[2]);
	}

	else if (command == "addSkeleton")
	{
		Commands::AddSkeleton(arguments[0], arguments[1], std::stoi(arguments[2]), arguments[3]);
	}

	else if (command == "removeSkeleton")
	{
		Commands::RemoveSkeleton(arguments[0], std::stoi(arguments[1]), arguments[2]);
	}

	else if (command == "listBones")
	{
		Commands::ListBones(arguments[0], std::stoi(arguments[1]));
	}

	else if (command == "toPackFile")
	{
		Commands::ToPackFile(arguments[0], arguments[1]);
	}

	else if (command == "toTagFile")
	{
		Commands::ToTagFile(arguments[0], arguments[1]);
	}

	else if (command == "toXMLFile")
	{
		Commands::ToXMLFile(arguments[0], arguments[1]);
	}

	else if (command == "compress")
	{
		const std::string& subCommand = arguments[0];

		if (subCommand == "quantized")
		{
			Commands::CompressQuanitized(arguments[1], std::stoi(arguments[2]), std::stoi(arguments[3]), std::stof(arguments[4]), std::stof(arguments[5]), std::stof(arguments[6]), std::stof(arguments[7]), arguments[8]);
		}
		else if (subCommand == "predictive")
		{
			Commands::CompressPredictive(arguments[1], std::stoi(arguments[2]), std::stoi(arguments[3]), std::stof(arguments[4]), std::stof(arguments[5]), std::stof(arguments[6]), std::stof(arguments[7]), std::stof(arguments[8]), std::stof(arguments[9]), std::stof(arguments[10]), std::stof(arguments[11]), arguments[12]);
		}
		else
		{
			throw FatalException("No compression subcommand '" + subCommand + "' found.");
		}
	}

	else if (command == "toFbxSkeleton")
	{
		Commands::ToFbxSkeleton(arguments[0], std::stoi(arguments[1]), arguments[2]);
	}

	else if (command == "toFbxAnimation")
	{
		Commands::ToFbxAnimation(arguments[0], std::stoi(arguments[1]), std::stoi(arguments[2]), arguments[3]);
	}

	else if (command == "listFbxStacks")
	{
		Commands::ListFBXStacks(arguments[0]);
	}

	else if (command == "fromFbxSkeleton")
	{
		Commands::FromFbxSkeleton(arguments[0], arguments[1], arguments[2], arguments[3]);
	}

	else if (command == "fromFbxAnimation")
	{
		Commands::FromFbxAnimation(arguments[0], arguments[1], std::stoi(arguments[2]), arguments[3], std::stoi(arguments[4]), arguments[5], arguments[6]);
	}

	else if (command == "listFbxBones")
	{
		Commands::ListFbxBones(arguments[0]);
	}

	else
	{
		throw FatalException("No command '" + command + "' found.");
	}

	FbxSystem::Instance().Shutdown();
	HavokSystem::Instance().Shutdown();
}

void Commands::GetStats(const std::string& containerPath)
{
	hkRootLevelContainer* rootContainer = HavokUtils::GetRootContainer(containerPath);
	hkaAnimationContainer* animationContainer = HavokUtils::GetAnimationContainer(rootContainer);

	std::cerr << animationContainer->m_skeletons.getSize() << ITEM_DELIM << animationContainer->m_animations.getSize() << ITEM_DELIM << animationContainer->m_bindings.getSize();
}

void Commands::CreateContainer(const std::string& containerPath)
{
	hkRootLevelContainer* rootContainer = HavokUtils::CreateBlankContainer();

	HavokUtils::SaveTagfile(containerPath, rootContainer);

	delete rootContainer;
}

void Commands::AddAnimation(const std::string& targetContainerPath, const std::string& sourceContainerPath, int animIdx, const std::string& outputContainerPath)
{
	hkRootLevelContainer* targetRootContainer = HavokUtils::GetRootContainer(targetContainerPath);
	hkaAnimationContainer* targetAnimationContainer = HavokUtils::GetAnimationContainer(targetRootContainer);

	hkRootLevelContainer* sourceRootContainer = HavokUtils::GetRootContainer(sourceContainerPath);
	hkaAnimationContainer* sourceAnimationContainer = HavokUtils::GetAnimationContainer(sourceRootContainer);

	hkaAnimation* anim = sourceAnimationContainer->m_animations[animIdx];

	hkaAnimationBinding* binding = nullptr;
	for (auto& currentBinding : sourceAnimationContainer->m_bindings)
	{
		if (currentBinding->m_animation == anim)
		{
			binding = currentBinding;
		}
	}

	targetAnimationContainer->m_animations.pushBack(anim);
	int newAnimIdx = targetAnimationContainer->m_animations.getSize() - 1;

	int newBindingIdx = -1;
	if (binding)
	{
		targetAnimationContainer->m_bindings.pushBack(binding);
		newBindingIdx = targetAnimationContainer->m_bindings.getSize() - 1;
	}

	HavokUtils::SaveTagfile(outputContainerPath, targetRootContainer);

	std::cerr << newAnimIdx << ITEM_DELIM << newBindingIdx;
}

void Commands::ReplaceAnimation(const std::string& targetContainerPath, int targetAnimIdx, const std::string& sourceContainerPath, int sourceAnimIdx, const std::string& outputContainerPath)
{
	hkRootLevelContainer* targetRootContainer = HavokUtils::GetRootContainer(targetContainerPath);
	hkaAnimationContainer* targetAnimationContainer = HavokUtils::GetAnimationContainer(targetRootContainer);

	hkRootLevelContainer* sourceRootContainer = HavokUtils::GetRootContainer(sourceContainerPath);
	hkaAnimationContainer* sourceAnimationContainer = HavokUtils::GetAnimationContainer(sourceRootContainer);

	hkaAnimation* sourceAnim = sourceAnimationContainer->m_animations[sourceAnimIdx];
	hkaAnimation* targetAnimation = targetAnimationContainer->m_animations[targetAnimIdx];

	hkaAnimationBinding* sourceBinding = nullptr;
	for (auto& currentBinding : sourceAnimationContainer->m_bindings)
	{
		if (currentBinding->m_animation == sourceAnim)
		{
			sourceBinding = currentBinding;
		}
	}

	targetAnimationContainer->m_animations[targetAnimIdx] = sourceAnim;

	int targetBindingIndex = -1;

	if (sourceBinding)
	{
		for (int i = targetAnimationContainer->m_bindings.getSize() - 1; i >= 0; --i)
		{
			hkaAnimationBinding* binding = targetAnimationContainer->m_bindings[i];
			if (binding->m_animation == targetAnimation)
			{
				targetBindingIndex = i;
			}
		}

		if (targetBindingIndex == -1)
		{
			targetAnimationContainer->m_bindings.pushBack(sourceBinding);
			targetBindingIndex = targetAnimationContainer->m_bindings.getSize();
		}
		else
		{
			targetAnimationContainer->m_bindings[targetBindingIndex] = sourceBinding;
		}
	}
	HavokUtils::SaveTagfile(outputContainerPath, targetRootContainer);

	std::cerr << targetAnimIdx << ITEM_DELIM << targetBindingIndex;
}

void Commands::RemoveAnimation(const std::string& targetContainerPath, int animIdx, const std::string& outputContainerPath)
{
	hkRootLevelContainer* targetRootContainer = HavokUtils::GetRootContainer(targetContainerPath);
	hkaAnimationContainer* targetAnimationContainer = HavokUtils::GetAnimationContainer(targetRootContainer);

	hkaAnimation* anim = targetAnimationContainer->m_animations[animIdx];
	targetAnimationContainer->m_animations.removeAt(animIdx);

	for (int i = targetAnimationContainer->m_bindings.getSize() - 1; i >= 0; --i)
	{
		hkaAnimationBinding* binding = targetAnimationContainer->m_bindings[i];
		if (binding->m_animation == anim)
		{
			targetAnimationContainer->m_bindings.removeAt(i);
		}
	}

	HavokUtils::SaveTagfile(outputContainerPath, targetRootContainer);

	std::cerr << targetAnimationContainer->m_animations.getSize() << ITEM_DELIM << targetAnimationContainer->m_bindings.getSize();
}

void Commands::AddSkeleton(const std::string& targetContainerPath, const std::string& sourceContainerPath, int skeletonIdx, const std::string& outputContainerPath)
{
	hkRootLevelContainer* targetRootContainer = HavokUtils::GetRootContainer(targetContainerPath);
	hkaAnimationContainer* targetAnimationContainer = HavokUtils::GetAnimationContainer(targetRootContainer);

	hkRootLevelContainer* sourceRootContainer = HavokUtils::GetRootContainer(sourceContainerPath);
	hkaAnimationContainer* sourceAnimationContainer = HavokUtils::GetAnimationContainer(sourceRootContainer);

	hkaSkeleton* skeleton = sourceAnimationContainer->m_skeletons[skeletonIdx];
	targetAnimationContainer->m_skeletons.pushBack(skeleton);

	HavokUtils::SaveTagfile(outputContainerPath, targetRootContainer);

	std::cerr << targetAnimationContainer->m_skeletons.getSize();
}

void Commands::RemoveSkeleton(const std::string& targetContainerPath, int skeletonIdx, const std::string& outputContainerPath)
{
	hkRootLevelContainer* targetRootContainer = HavokUtils::GetRootContainer(targetContainerPath);
	hkaAnimationContainer* targetAnimationContainer = HavokUtils::GetAnimationContainer(targetRootContainer);

	targetAnimationContainer->m_skeletons.removeAt(skeletonIdx);

	HavokUtils::SaveTagfile(outputContainerPath, targetRootContainer);

	std::cerr << targetAnimationContainer->m_skeletons.getSize();
}

void Commands::ListBones(const std::string& containerPath, int skeletonIdx)
{
	hkRootLevelContainer* rootContainer = HavokUtils::GetRootContainer(containerPath);
	hkaAnimationContainer* animationContainer = HavokUtils::GetAnimationContainer(rootContainer);

	hkaSkeleton* skeleton = animationContainer->m_skeletons[skeletonIdx];
	int boneCount = skeleton->m_bones.getSize();

	for (int i = 0; i < boneCount; ++i)
	{
		std::cerr << skeleton->m_bones[i].m_name;
		if (i < boneCount - 1)
			std::cerr << ITEM_DELIM;
	}
}

void Commands::ToPackFile(const std::string& sourceContainerPath, const std::string& outputContainerPath)
{
	hkRootLevelContainer* rootContainer = HavokUtils::GetRootContainer(sourceContainerPath);
	HavokUtils::SavePackFile(outputContainerPath, rootContainer);
}

void Commands::ToTagFile(const std::string& sourceContainerPath, const std::string& outputContainerPath)
{
	hkRootLevelContainer* rootContainer = HavokUtils::GetRootContainer(sourceContainerPath);
	HavokUtils::SaveTagfile(outputContainerPath, rootContainer);
}

void Commands::ToXMLFile(const std::string& sourceContainerPath, const std::string& outputContainerPath)
{
	hkRootLevelContainer* rootContainer = HavokUtils::GetRootContainer(sourceContainerPath);
	HavokUtils::SaveXMLFile(outputContainerPath, rootContainer);
}

void Commands::CompressQuanitized(const std::string& sourceContainerPath, int sourceAnimIdx, int sourceSkeletonIdx, float floatingTolerance, float translationTolerance, float rotationTolerance, float scaleTolerance, const std::string& outputContainerPath)
{
	hkRootLevelContainer* rootContainer = HavokUtils::GetRootContainer(sourceContainerPath);
	hkaAnimationContainer* animationContainer = HavokUtils::GetAnimationContainer(rootContainer);

	hkaAnimation* sourceAnimation = animationContainer->m_animations[sourceAnimIdx];
	hkaSkeleton* sourceSkeleton = animationContainer->m_skeletons[sourceSkeletonIdx];

	if (HavokUtils::IsCompressed(sourceAnimation))
		throw FatalException("Animation is already compressed.");

	auto bindingIt = std::find_if(animationContainer->m_bindings.begin(), animationContainer->m_bindings.end(), [&](hkaAnimationBinding* binding) { return binding->m_animation == sourceAnimation; });
	if (bindingIt == animationContainer->m_bindings.end())
		throw FatalException("Could not find binding");
	hkaAnimationBinding* sourceBinding = *bindingIt;

	hkaQuantizedAnimation::TrackCompressionParams params;
	params.m_floatingTolerance = floatingTolerance;
	params.m_translationTolerance = translationTolerance;
	params.m_rotationTolerance = rotationTolerance;
	params.m_scaleTolerance = scaleTolerance;
	
	hkaQuantizedAnimation* newAnim = new hkaQuantizedAnimation(*sourceBinding, *sourceSkeleton, params);
	sourceBinding->m_animation = newAnim;

	hkRootLevelContainer* newRootContainer = HavokUtils::CreateBlankContainer();
	hkaAnimationContainer* newAnimContainer = HavokUtils::GetAnimationContainer(newRootContainer);

	newAnimContainer->m_bindings.pushBack(sourceBinding);
	newAnimContainer->m_animations.pushBack(newAnim);

	HavokUtils::SaveTagfile(outputContainerPath, newRootContainer);
}

void Commands::CompressPredictive(const std::string& sourceContainerPath, int sourceAnimIdx, int sourceSkeletonIdx, float staticFloatingTolerance, float staticTranslationTolerance, float staticRotationTolerance, float staticScaleTolerance, float dynamicFloatingTolerance, float dynamicTranslationTolerance, float dynamicRotationTolerance, float dynamicScaleTolerance, const std::string& outputContainerPath)
{
	hkRootLevelContainer* rootContainer = HavokUtils::GetRootContainer(sourceContainerPath);
	hkaAnimationContainer* animationContainer = HavokUtils::GetAnimationContainer(rootContainer);

	hkaAnimation* sourceAnimation = animationContainer->m_animations[sourceAnimIdx];
	hkaSkeleton* sourceSkeleton = animationContainer->m_skeletons[sourceSkeletonIdx];

	if(HavokUtils::IsCompressed(sourceAnimation))
		throw FatalException("Animation is already compressed.");

	auto bindingIt = std::find_if(animationContainer->m_bindings.begin(), animationContainer->m_bindings.end(), [&](hkaAnimationBinding* binding) { return binding->m_animation == sourceAnimation; });
	if (bindingIt == animationContainer->m_bindings.end())
		throw FatalException("Could not find binding");
	hkaAnimationBinding* sourceBinding = *bindingIt;

	hkaPredictiveCompressedAnimation::CompressionParams params(staticTranslationTolerance, staticRotationTolerance, staticScaleTolerance, staticFloatingTolerance, dynamicTranslationTolerance, dynamicRotationTolerance, dynamicScaleTolerance, dynamicFloatingTolerance);

	hkaPredictiveCompressedAnimation* newAnim = new hkaPredictiveCompressedAnimation(*sourceBinding, *sourceSkeleton, params);

	sourceBinding->m_animation = newAnim;

	hkRootLevelContainer* newRootContainer = HavokUtils::CreateBlankContainer();
	hkaAnimationContainer* newAnimContainer = HavokUtils::GetAnimationContainer(newRootContainer);

	newAnimContainer->m_bindings.pushBack(sourceBinding);
	newAnimContainer->m_animations.pushBack(newAnim);

	HavokUtils::SaveTagfile(outputContainerPath, newRootContainer);
}

void Commands::ToFbxSkeleton(const std::string& container, int skeletonIdx, const std::string& outputFbx)
{
	hkRootLevelContainer* rootContainer = HavokUtils::GetRootContainer(container);
	hkaAnimationContainer* animationContainer = HavokUtils::GetAnimationContainer(rootContainer);

	hkaSkeleton* skeleton = animationContainer->m_skeletons[skeletonIdx];
	FbxScene* scene = FbxSystem::Instance().CreateScene("XATScene");

	int bonesConverted = HavokToFbx::ConvertSkeleton(skeleton, scene);

	FbxSystem::Instance().SaveScene(scene, outputFbx.c_str());

	std::cerr << bonesConverted;
}

void Commands::ToFbxAnimation(const std::string& container, int skeletonIdx, int animationIdx, const std::string& outputFbx)
{
	hkRootLevelContainer* rootContainer = HavokUtils::GetRootContainer(container);
	hkaAnimationContainer* animationContainer = HavokUtils::GetAnimationContainer(rootContainer);

	hkaSkeleton* skeleton = animationContainer->m_skeletons[skeletonIdx];
	FbxScene* scene = FbxSystem::Instance().CreateScene("XATScene");

	int bonesConverted = HavokToFbx::ConvertSkeleton(skeleton, scene);

	hkaAnimation* animation = animationContainer->m_animations[skeletonIdx];

	auto bindingIt = std::find_if(animationContainer->m_bindings.begin(), animationContainer->m_bindings.end(), [&](hkaAnimationBinding* binding) { return binding->m_animation == animation; });
	if (bindingIt == animationContainer->m_bindings.end())
		throw FatalException("Could not find binding");
	hkaAnimationBinding* binding = *bindingIt;

	int framesConverted = HavokToFbx::ConvertAnimation(animation, binding, skeleton, scene);

	FbxSystem::Instance().SaveScene(scene, outputFbx.c_str());

	std::cerr << bonesConverted << ITEM_DELIM << framesConverted;
}

void Commands::ListFBXStacks(const std::string& fbxPath)
{
	FbxScene* scene = FbxSystem::Instance().CreateScene("XATScene");
	FbxSystem::Instance().LoadScene(scene, fbxPath.c_str());

	int numStacks = scene->GetSrcObjectCount(FbxCriteria::ObjectType(FbxAnimStack::ClassId));
	for (int i = 0; i < numStacks; ++i)
	{
		FbxAnimStack* animStack = FbxCast<FbxAnimStack>(scene->GetSrcObject(FbxCriteria::ObjectType(FbxAnimStack::ClassId), i));

		std::cerr << animStack->GetName();
		if (i < numStacks - 1)
			std::cerr << ITEM_DELIM;
	}

	scene->Destroy();
}

void Commands::FromFbxSkeleton(const std::string& targetContainerPath, const std::string& fbxPath, const std::string& boneOrder, const std::string& outputContainerPath)
{
	hkRootLevelContainer* rootContainer = HavokUtils::GetRootContainer(targetContainerPath.c_str());
	hkaAnimationContainer* animationContainer = HavokUtils::GetAnimationContainer(rootContainer);

	FbxScene* scene = FbxSystem::Instance().CreateScene("NewSkeleton");

	FbxSystem::Instance().LoadScene(scene, fbxPath.c_str());

	hkaSkeleton* skeleton = new hkaSkeleton();

	auto boneOrderList = IOUtils::SplitString(boneOrder, ",");

	int bonesConverted = FbxToHavok::ConvertSkeleton(scene, skeleton, boneOrderList);
	animationContainer->m_skeletons.pushBack(skeleton);

	HavokUtils::SaveTagfile(outputContainerPath, rootContainer);

	std::cerr << bonesConverted;

	scene->Destroy();
}

void Commands::FromFbxAnimation(const std::string& targetContainerPath, const std::string& fbxPath, int animStackIdx, const std::string& sourceSkeleton, int skeletonIdx, const std::string& excludeBones, const std::string& outputContainerPath)
{
	hkRootLevelContainer* sourceRootContainer = HavokUtils::GetRootContainer(sourceSkeleton);
	hkaAnimationContainer* sourceAnimationContainer = HavokUtils::GetAnimationContainer(sourceRootContainer);
	hkaSkeleton* skeleton = sourceAnimationContainer->m_skeletons[skeletonIdx];

	FbxScene* scene = FbxSystem::Instance().CreateScene("NewAnimation");

	FbxSystem::Instance().LoadScene(scene, fbxPath.c_str());

	FbxAnimStack* animStack = FbxCast<FbxAnimStack>(scene->GetSrcObject(FbxCriteria::ObjectType(FbxAnimStack::ClassId), animStackIdx));

	hkaInterleavedUncompressedAnimation* animation = new hkaInterleavedUncompressedAnimation();
	hkaAnimationBinding* binding = new hkaAnimationBinding();

	auto excludedBones = IOUtils::SplitString(excludeBones, ",");

	int framesConverted = FbxToHavok::ConvertAnimation(scene, animStack, skeleton, animation, binding, excludedBones);

	hkRootLevelContainer* targetRootContainer = HavokUtils::GetRootContainer(targetContainerPath.c_str());
	hkaAnimationContainer* targetAnimationContainer = HavokUtils::GetAnimationContainer(targetRootContainer);

	targetAnimationContainer->m_animations.pushBack(animation);
	targetAnimationContainer->m_bindings.pushBack(binding);

	HavokUtils::SaveTagfile(outputContainerPath, targetRootContainer);

	std::cerr << framesConverted << ITEM_DELIM << binding->m_transformTrackToBoneIndices.getSize();

	scene->Destroy();
}

void Commands::ListFbxBones(const std::string& fbxPath)
{
	FbxScene* scene = FbxSystem::Instance().CreateScene("NewSkeleton");

	FbxSystem::Instance().LoadScene(scene, fbxPath.c_str());

	FbxBoneMap boneMap = FbxBoneMap::Create(scene);

	int boneCount = boneMap.data.size();

	for (int i = 0; i < boneCount; ++i)
	{
		std::cerr << boneMap.GetNodeAtIndex(i)->GetName();
		if (i < boneCount - 1)
			std::cerr << ITEM_DELIM;
	}

	scene->Destroy();
}
