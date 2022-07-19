#pragma once

#include <string>

#include "HavokHeaders.hpp"

class HavokUtils
{
public:
	static hkRootLevelContainer* GetRootContainer(const std::string& containerPath);
	static hkaAnimationContainer* GetAnimationContainer(hkRootLevelContainer* rootContainer);
	static hkRootLevelContainer* CreateBlankContainer();

	static void SaveTagfile(const std::string& containerPath, hkRootLevelContainer* rootContainer);
	static void SaveXMLFile(const std::string& containerPath, hkRootLevelContainer* rootContainer);
	static void SavePackFile(const std::string& containerPath, hkRootLevelContainer* rootContainer);

	static bool IsCompressed(hkaAnimation* animation);
};