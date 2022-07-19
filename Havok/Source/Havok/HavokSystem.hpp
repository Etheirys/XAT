#pragma once

#include "../Utils/Singleton.hpp"

class HavokSystem : public Singleton<HavokSystem>
{
public:
	void Init();

	void Shutdown();

	class hkLoader* GetLoader() { return m_loader; }

private:
	class hkLoader* m_loader;
};