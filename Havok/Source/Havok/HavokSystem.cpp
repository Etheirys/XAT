#include <iostream>

#include "HavokSystem.hpp"
#include "HavokHeaders.hpp"

#include <Common/Base/System/Init/PlatformInit.cxx>

static void HK_CALL havokErrorReport(const char* msg, void* userContext) {
    std::cout << "Havok Output: " << msg << std::endl;
}

void HavokSystem::Init()
{
    PlatformInit();
    hkMemoryRouter* memoryRouter = hkMemoryInitUtil::initDefault(hkMallocAllocator::m_defaultMallocAllocator, hkMemorySystem::FrameInfo(1024 * 1024));
    hkBaseSystem::init(memoryRouter, havokErrorReport);
    PlatformFileSystemInit();
    hkSerializeDeprecatedInit::initDeprecated();

    m_loader = new hkLoader();
}

void HavokSystem::Shutdown()
{
    delete m_loader;

    hkBaseSystem::quit();
}

#include <Common/Base/keycode.cxx>

#undef HK_FEATURE_PRODUCT_AI
//#undef HK_FEATURE_PRODUCT_ANIMATION
#undef HK_FEATURE_PRODUCT_CLOTH
#undef HK_FEATURE_PRODUCT_DESTRUCTION_2012
#undef HK_FEATURE_PRODUCT_DESTRUCTION
#undef HK_FEATURE_PRODUCT_BEHAVIOR
#undef HK_FEATURE_PRODUCT_PHYSICS_2012
#undef HK_FEATURE_PRODUCT_SIMULATION
#undef HK_FEATURE_PRODUCT_PHYSICS

#define HK_SERIALIZE_MIN_COMPATIBLE_VERSION 201130r1

#include <Common/Base/Config/hkProductFeatures.cxx>