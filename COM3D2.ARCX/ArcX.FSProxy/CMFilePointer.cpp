#include "CMFilePointer.h"
#include <fstream>

#define LOG_MSG(msg) *(this->data->logger) << msg << std::endl;

CMFilePointer::CMFilePointer(ArcXArchiveData *data, std::wstring path)
{
	this->data = data;
	this->stream = std::ifstream(path, std::ios::binary);

	this->stream.seekg(0, std::ios::end);
	this->len = stream.tellg();
	this->stream.seekg(0, std::ios::beg);

	LOG_MSG("Created file pointer!");
}

CMFilePointer *CMFilePointer::dispose(bool disposing)
{
	LOG_MSG("Disposing!");

	this->stream.close();

	if (disposing)
		delete this;
	return this;
}

bool CMFilePointer::close_file()
{
	LOG_MSG("Closing file!");
	this->stream.close();
	return false;
}

bool CMFilePointer::seek(uint64_t dist, bool absolute)
{
	LOG_MSG("Seeking data!");
	this->stream.seekg(dist, absolute ? std::ios::beg : std::ios::cur);
	return true;
}

uint64_t CMFilePointer::read(void *dest, uint64_t length)
{
	LOG_MSG("Reading data!");
	this->stream.read((char*) dest, length);
	return this->stream.gcount();
}

uint64_t CMFilePointer::read_from(void *buffer, uint64_t pos, uint64_t length)
{
	LOG_MSG("Reading data from pos!");
	size_t prev = this->stream.tellg();
	this->stream.seekg(pos, std::ios::beg);
	this->stream.read((char*)buffer, length);
	size_t read = stream.gcount();
	this->stream.seekg(prev, std::ios::beg);
	return read;
}

bool CMFilePointer::is_open()
{
	LOG_MSG("Checking if open!");
	return this->stream.is_open();
}

uint64_t CMFilePointer::tell()
{
	LOG_MSG("Telling position");
	return this->stream.tellg();
}

void *CMFilePointer::next_file()
{
	LOG_MSG("Getting next file");
	return nullptr;
}

uint64_t CMFilePointer::unk1()
{
	LOG_MSG("Unknown1");
	return this->len;
}

uint64_t CMFilePointer::length()
{
	LOG_MSG("Getting length");
	return this->len;
}

bool CMFilePointer::set_file(void *data, uint64_t data_length, uint64_t file_offset)
{
	LOG_MSG("Setting file");
	return false;
}

bool CMFilePointer::set_file2(void *data, uint64_t data_length, uint64_t file_offset)
{
	LOG_MSG("Setting file2");
	return false;
}

size_t CMFilePointer::move_file_ptr(void *dest, void *src, size_t len)
{
	LOG_MSG("Moving file ptr");
	return 0;
}
