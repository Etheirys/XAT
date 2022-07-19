#include "FbxSystem.hpp"

#include "../FatalException.hpp"

#include <iostream>

FbxSystem::FbxSystem() : 
    m_manager(nullptr)
{

}

void FbxSystem::Init()
{
    m_manager = FbxManager::Create();
    if (!m_manager)
        throw FatalException("Failed to initialize FbxManager");
}

void FbxSystem::Shutdown()
{
    if (m_manager)
        m_manager->Destroy();
}

FbxManager& FbxSystem::GetManager()
{
    if(!m_manager)
        throw FatalException("FbxSystem is not initalized");

    return *m_manager;
}

FbxScene* FbxSystem::CreateScene(const char* name)
{
    FbxManager& manager = GetManager();

    FbxScene* scene = FbxScene::Create(&manager, name);

    if(!scene)
        throw FatalException("Failed to initialize FbxScene");

    scene->GetGlobalSettings().SetTimeMode(FbxTime::eFrames30);
    scene->GetGlobalSettings().SetCustomFrameRate(30.0);
    scene->GetGlobalSettings().SetSystemUnit(FbxSystemUnit::m);

    return scene;
}

void FbxSystem::LoadScene(FbxScene* scene, const char* fileName)
{
    FbxManager& manager = GetManager();

    FbxImporter* importer = FbxImporter::Create(&manager, "");
    bool importStatus = importer->Initialize(fileName, -1, manager.GetIOSettings());

    if (!importStatus)
    {
        FbxString error = importer->GetStatus().GetErrorString();
        std::string errorStr(error);
        throw FatalException("FBX Import Error: " + errorStr);
    }

    if (!importer->IsFBX())
        throw FatalException("Importer is not FBX");

    bool loadStatus = importer->Import(scene);

    if (!loadStatus) {
        FbxString error = importer->GetStatus().GetErrorString();
        std::string errorStr(error);
        throw FatalException("FBX Import Error: " + errorStr);
    }

    importer->Destroy();
}

void FbxSystem::SaveScene(FbxScene* scene, const char* fileName)
{
    FbxManager& manager = GetManager();

    FbxExporter* exporter = FbxExporter::Create(&manager, "");

    int fileFormat = manager.GetIOPluginRegistry()->FindWriterIDByDescription("FBX binary (*.fbx)");

    bool exporterStatus = exporter->Initialize(fileName, fileFormat, manager.GetIOSettings());

    if (!exporterStatus) {
        FbxString error = exporter->GetStatus().GetErrorString();
        std::string errorStr(error);
        throw FatalException("Export Error: " + errorStr);
    }

    bool exportStatus = exporter->Export(scene);

    if (!exportStatus) {
        FbxString error = exporter->GetStatus().GetErrorString();
        std::string errorStr(error);
        throw FatalException("Export Error: " + errorStr);
    }

    exporter->Destroy();
}
