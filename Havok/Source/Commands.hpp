#pragma once

#include <string>
#include <vector>

class Commands
{
public:
	static void Run(const std::string& command, const std::vector<std::string>& arguments);

private:
	static void GetStats(const std::string& containerPath);
	static void CreateContainer(const std::string& containerPath);
	static void AddAnimation(const std::string& targetContainerPath, const std::string& sourceContainerPath, int animIdx, const std::string& outputContainerPath);
	static void ReplaceAnimation(const std::string& targetContainerPath, int targetAnimIdx, const std::string& sourceContainerPath, int sourceAnimIdx, const std::string& outputContainerPath);
	static void RemoveAnimation(const std::string& targetContainerPath, int animIdx, const std::string& outputContainerPath);
	static void AddSkeleton(const std::string& targetContainerPath, const std::string& sourceContainerPath, int skeletonIdx, const std::string& outputContainerPath);
	static void RemoveSkeleton(const std::string& targetContainerPath, int skeletonIdx, const std::string& outputContainerPath);
	static void ListBones(const std::string& containerPath, int skeletonIdx);
	static void ToPackFile(const std::string& sourceContainerPath, const std::string& outputContainerPath);
	static void ToTagFile(const std::string& sourceContainerPath, const std::string& outputContainerPath);
	static void ToXMLFile(const std::string& sourceContainerPath, const std::string& outputContainerPath);
	static void CompressQuanitized(const std::string& sourceContainerPath, int sourceAnimIdx, int sourceSkeletonIdx, float floatingTolerance, float translationTolerance, float rotationTolerance, float scaleTolerance, const std::string& outputContainerPath);
	static void CompressPredictive(const std::string& sourceContainerPath, int sourceAnimIdx, int sourceSkeletonIdx, float staticFloatingTolerance, float staticTranslationTolerance, float staticRotationTolerance, float staticScaleTolerance, float dynamicFloatingTolerance, float dynamicTranslationTolerance, float dynamicRotationTolerance, float dynamicScaleTolerance, const std::string& outputContainerPath);
	static void ToFbxSkeleton(const std::string& container, int skeletonIdx, const std::string& outputFbx);
	static void ToFbxAnimation(const std::string& container, int skeletonIdx, int animationId, const std::string& outputFbx);
	static void ListFBXStacks(const std::string& fbxPath);
	static void FromFbxSkeleton(const std::string& targetContainerPath, const std::string& fbxPath, const std::string& boneOrder, const std::string& outputContainerPath);
	static void FromFbxAnimation(const std::string& targetContainerPath, const std::string& fbxPath, int animStackIdx, const std::string& sourceSkeleton, int skeletonIdx, const std::string& excludeBones, const std::string& outputContainerPath);
	static void ListFbxBones(const std::string& fbxPath);

};