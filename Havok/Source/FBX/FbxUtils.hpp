#pragma once

#include <string>
#include <utility>
#include <vector>
#include <fbxsdk.h>

class FbxUtils
{

};

class FbxBoneMap
{
public:
	FbxNode* GetNodeByName(const char* name);
	int GetIndexByName(const char* name);
	FbxNode* GetNodeAtIndex(int index);
	int Size();

	static FbxBoneMap Create(FbxScene* scene);

public:
	const FbxScene* scene;
	std::vector<std::pair<std::string, FbxNode*>> data;
};