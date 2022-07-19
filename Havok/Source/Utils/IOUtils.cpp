#include "IOUtils.hpp"

#include <iostream>
#include <fstream>

int IOUtils::FileSize(std::string const& filePath)
{
    std::streampos fsize = 0;
    std::ifstream file(filePath, std::ios::binary);

    fsize = file.tellg();
    file.seekg(0, std::ios::end);
    fsize = file.tellg() - fsize;
    file.close();

    return (int) fsize;
}

void IOUtils::ReadStreamIntoVector(std::vector<char>& vec, std::ifstream& stream, int count) {
    vec.clear();
    vec.reserve(count);
    for (int i = 0; i < count; i++) {
        char c;
        stream.read((char*)&c, 1);
        vec.push_back(c);
    }
}

void IOUtils::WriteVectorIntoStream(std::vector<char>& vec, std::ofstream& stream, int count) {
    for (int i = 0; i < count; i++) {
        stream.write(&vec[i], 1);
    }
}

void IOUtils::WriteIntToVector(std::vector<char>& buffer, int position, int value) {
    auto bytes = static_cast<char*>(static_cast<void*>(&value));
    for (int i = 0; i < sizeof(int); i++) {
        buffer[position + i] = bytes[i];
    }
}

std::vector<std::string> IOUtils::SplitString(const std::string& input, const std::string& delim)
{
    std::vector<std::string> output;

    auto start = 0U;
    auto end = input.find(delim);
    while (end != std::string::npos)
    {
        output.push_back(input.substr(start, end - start));
        start = end + delim.length();
        end = input.find(delim, start);
    }

    std::string leftOver = input.substr(start, end);
    if(leftOver != "")
        output.push_back(leftOver);

    return output;
}
