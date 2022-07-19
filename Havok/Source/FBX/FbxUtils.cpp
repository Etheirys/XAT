#include "FbxUtils.hpp"

#include <algorithm>

FbxNode* FbxBoneMap::GetNodeByName(const char* name)
{
	auto findIt = std::find_if(data.begin(), data.end(), [&](const std::pair<std::string, FbxNode*>& pair) { return pair.first == std::string(name); });
	if (findIt == data.end())
		return nullptr;

	return (*findIt).second;
}

int FbxBoneMap::GetIndexByName(const char* name)
{
	auto findIt = std::find_if(data.begin(), data.end(), [&](const std::pair<std::string, FbxNode*>& pair) { return pair.first == std::string(name); });
	if (findIt == data.end())
		return -1;

	auto distance = std::distance(data.begin(), findIt);

	return distance;
}

FbxNode* FbxBoneMap::GetNodeAtIndex(int index)
{
	return data[index].second;
}

int FbxBoneMap::Size()
{
	return data.size();
}

FbxBoneMap FbxBoneMap::Create(FbxScene* scene)
{
	FbxBoneMap map;
	map.scene = scene;

	int nodeCount = scene->GetNodeCount();
	for (int i = 0; i < nodeCount; ++i)
	{
		FbxNode* node = scene->GetNode(i);
		std::string nodeName = std::string(node->GetName());

		FbxNodeAttribute* attrib = node->GetNodeAttribute();

		if (attrib)
		{
			FbxNodeAttribute::EType attributeType = attrib->GetAttributeType();
			if (attributeType == FbxNodeAttribute::eSkeleton || (attributeType == FbxNodeAttribute::eNull)) // Blender has a bug where it strips eSkeleton from roots
			{
				map.data.push_back(std::make_pair(nodeName, node));
			}
		}
	}

	return map;
}
