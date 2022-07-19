#pragma once

#include <exception>
#include <string>

struct FatalException : public std::exception
{
public:
	FatalException(std::string ss) : s(ss) {}
	~FatalException() throw () {}
	const char* what() const throw() { return s.c_str(); }

private:
	std::string s;
};