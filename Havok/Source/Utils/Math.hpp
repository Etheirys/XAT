#pragma once

#include <fbxsdk.h>
#include "../Havok/HavokHeaders.hpp"

class Vector4
{
public:
	Vector4(float x, float y, float z, float w);

public:
	static Vector4 From(const hkVector4& hkVec);
	static Vector4 From(const hkQuaternion& hkVec);
	static Vector4 From(const FbxVector4& fbxVec);
	static Vector4 From(const FbxQuaternion& fbxQuat);

	Vector4 ToEuler();
	Vector4 ToQuat();

	hkVector4 ToHkVector4();
	hkQuaternion ToHkQuaternion();
	FbxVector4 ToFbxVector4();
	FbxQuaternion ToFbxQuaternion();

public:
	float x;
	float y;
	float z;
	float w;
};

class Math
{
public:
	static float Deg2Rad(float input);
	static float Rad2Deg(float input);
};