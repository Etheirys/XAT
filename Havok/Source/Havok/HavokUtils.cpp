#include "HavokUtils.hpp"
#include "HavokSystem.hpp"

#include "../FatalException.hpp"

hkRootLevelContainer* HavokUtils::GetRootContainer(const std::string& containerPath)
{
    hkRootLevelContainer* rootContainer = HavokSystem::Instance().GetLoader()->load(containerPath.c_str());

    if (!rootContainer)
        throw FatalException("Couldn't load container " + containerPath);

    return rootContainer;
}


hkaAnimationContainer* HavokUtils::GetAnimationContainer(hkRootLevelContainer* rootContainer)
{
    hkaAnimationContainer* animationContainer = reinterpret_cast<hkaAnimationContainer*>(rootContainer->findObjectByType(hkaAnimationContainerClass.getName()));
    if (!rootContainer)
        throw FatalException("Couldn't find animation container");

    return animationContainer;
}

hkRootLevelContainer* HavokUtils::CreateBlankContainer()
{
    hkRootLevelContainer* rootContainer = new hkRootLevelContainer();
    hkaAnimationContainer* animContainer = new hkaAnimationContainer();

    hkRootLevelContainer::NamedVariant variant(hkaAnimationContainerClass.getName(), animContainer, &hkaAnimationContainer::staticClass());
    rootContainer->m_namedVariants.pushBack(variant);

    return rootContainer;
}

void HavokUtils::SaveTagfile(const std::string& containerPath, hkRootLevelContainer* rootContainer)
{
    hkOstream stream(containerPath.c_str());

    hkResult res = hkSerializeUtil::saveTagfile(rootContainer, hkRootLevelContainer::staticClass(), stream.getStreamWriter(), nullptr,  hkSerializeUtil::SAVE_DEFAULT);

    if (!res.isSuccess())
        throw FatalException("Failed to save tagfile to " + containerPath);
}

void HavokUtils::SaveXMLFile(const std::string& containerPath, hkRootLevelContainer* rootContainer)
{
    hkOstream stream(containerPath.c_str());
	hkResult res = hkSerializeUtil::saveTagfile(rootContainer, hkRootLevelContainer::staticClass(), stream.getStreamWriter(), nullptr,  hkSerializeUtil::SAVE_TEXT_FORMAT);

    if (!res.isSuccess())
        throw FatalException("Failed to save xml tagfile to " + containerPath);
}

void HavokUtils::SavePackFile(const std::string& containerPath, hkRootLevelContainer* rootContainer)
{
    hkStructureLayout::LayoutRules layoutRules = hkStructureLayout::HostLayoutRules;
    layoutRules.m_bytesInPointer = 8;

    hkPackfileWriter::Options packOptions;
    packOptions.m_layout = layoutRules;
    layoutRules.m_littleEndian = true;

    hkOstream stream(containerPath.c_str());
    hkResult res = hkSerializeUtil::savePackfile(rootContainer, hkRootLevelContainer::staticClass(), stream.getStreamWriter(), packOptions, nullptr,  hkSerializeUtil::SAVE_DEFAULT);

    if (!res.isSuccess())
        throw FatalException("Failed to save packfile to " + containerPath);
}

bool HavokUtils::IsCompressed(hkaAnimation* animation)
{
    switch (animation->getType())
    {
        case hkaAnimation::AnimationType::HK_PREDICTIVE_COMPRESSED_ANIMATION:
        case hkaAnimation::AnimationType::HK_QUANTIZED_COMPRESSED_ANIMATION:
        case hkaAnimation::AnimationType::HK_SPLINE_COMPRESSED_ANIMATION:
            return true;
    }

    return false;
}

