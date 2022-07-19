#pragma once

#include <memory>
#include <fbxsdk.h>

#include "../Utils/Singleton.hpp"

class FbxSystem : public Singleton<FbxSystem>
{
public:
	FbxSystem();

	void Init();

	void Shutdown();

	FbxManager& GetManager();

	FbxScene* CreateScene(const char* name);
	void LoadScene(class FbxScene* scene, const char* fileName);
	void SaveScene(class FbxScene* scene, const char* fileName);

private:
	FbxManager* m_manager;
};