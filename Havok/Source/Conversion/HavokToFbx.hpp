#pragma once

#include "../Havok/HavokHeaders.hpp"
#include <fbxsdk.h>

class HavokToFbx
{
public:
	static int ConvertSkeleton(hkaSkeleton* sourceSkeleton, FbxScene* targetScene);

	static int ConvertAnimation(hkaAnimation* sourceAnimation, hkaAnimationBinding* sourceBinding, hkaSkeleton* sourceSkeleton, FbxScene* targetScene);
};