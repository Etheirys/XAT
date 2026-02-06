#pragma once

#include <string>
#include <vector>
#include <fstream>

class IOUtils
{
public:
	static void ReadStreamIntoVector(std::vector<char>& vec, std::ifstream& stream, int count);
	static void WriteVectorIntoStream(std::vector<char>& vec, std::ofstream& stream, int count);

	static void WriteIntToVector(std::vector<char>& buffer, int position, int value);

	static std::vector<std::string> SplitString(const std::string& input, const std::string& delim);
};