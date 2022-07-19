#include "Math.hpp"

#include "../Havok/HavokHeaders.hpp"
#include <fbxsdk.h>
#include <Euler/EulerAngles.h>

#define M_PI 3.14159265358979323846f

Vector4::Vector4(float x, float y, float z, float w) :
	x(x),
	y(y),
	z(z),
	w(w)
{

}

Vector4 Vector4::From(const hkVector4& hkVec)
{
	return Vector4(hkVec.getSimdAt(0), hkVec.getSimdAt(1), hkVec.getSimdAt(2), hkVec.getSimdAt(3));
}


Vector4 Vector4::From(const hkQuaternion& hkQuat)
{
	return Vector4(hkQuat.m_vec.getSimdAt(0), hkQuat.m_vec.getSimdAt(1), hkQuat.m_vec.getSimdAt(2), hkQuat.m_vec.getSimdAt(3));
}

Vector4 Vector4::From(const FbxVector4& fbxVec)
{
	return Vector4((float)fbxVec.mData[0], (float)fbxVec.mData[1], (float)fbxVec.mData[2], (float)fbxVec.mData[3]);
}

Vector4 Vector4::From(const FbxQuaternion& fbxQuat)
{
	return Vector4((float)fbxQuat.mData[0], (float)fbxQuat.mData[1], (float)fbxQuat.mData[2], (float)fbxQuat.mData[3]);
}

Vector4 Vector4::ToEuler()
{
	Quat converQuat = { x, y, z, w };
	EulerAngles eulerAngles = Eul_FromQuat(converQuat, EulOrdXYZs);
	return Vector4(Math::Rad2Deg((float)eulerAngles.x), Math::Rad2Deg((float)eulerAngles.y), Math::Rad2Deg((float)eulerAngles.z), 0.0f);
}

Vector4 Vector4::ToQuat()
{
	EulerAngles eulerAngles = { Math::Deg2Rad(x), Math::Deg2Rad(y), Math::Deg2Rad(z), Math::Deg2Rad(w) };
	Quat quat = Eul_ToQuat(eulerAngles);
	return Vector4((float) quat.x, (float) quat.y, (float) quat.z, (float)quat.w);
}

hkVector4 Vector4::ToHkVector4()
{
	return hkVector4(x, y, z, w);
}

hkQuaternion Vector4::ToHkQuaternion()
{
	return hkQuaternion(x, y, z, w);
}

FbxVector4 Vector4::ToFbxVector4()
{
	return FbxVector4(x, y, z, w);
}

FbxQuaternion Vector4::ToFbxQuaternion()
{
	return FbxQuaternion(x, y, z, w);
}

float Math::Deg2Rad(float input)
{
	
	return (input * (M_PI / 180));
}

float Math::Rad2Deg(float input)
{
	return (input * (180.0f / M_PI));
}
