#pragma once

template<typename T>
class Singleton
{
public:
	static T& Instance()
	{
		static T* t = new T();
		return *t;
	}

protected:
	Singleton() {}
	~Singleton() {}

private:
	Singleton(Singleton const&);
};