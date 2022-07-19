#include <iostream>
#include <vector>

#include "FatalException.hpp"
#include "Commands.hpp"

int main(int argc, char* argv[])
{
	if (argc <= 1)
	{
		std::cout << "Error: No arguments provided. Do not run this directly.";
		return 1;
	}

	std::string command = argv[1];
	std::vector<std::string> arguments;

	if (argc >= 3)
	{
		for (int i = 2; i < argc; ++i)
		{
			arguments.push_back(argv[i]);
		}
	}
	
	try
	{
		Commands::Run(command, arguments);
	}
	catch (FatalException const& ex)
	{
		std::cout << ex.what() << std::endl;
		return 2;
	}
	catch (const std::exception& ex)
	{
		std::cout << "Internal Execution Error: " << ex.what() << std::endl;
		return 3;
	}
	catch (...)
	{
		std::cout << "Unknown Error" << std::endl;
		return 3;
	}

	return 0;
}