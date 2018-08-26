cmake_minimum_required(VERSION 3.0.0)
project (lib-i2c.native)

# The version number
set (LibI2C_VERSION_MAJOR 1)
set (LibI2C_VERSION_MINOR 0)

enable_language(C)
enable_language(CXX)

set (CMAKE_RUNTIME_OUTPUT_DIRECTORY ${PROJECT_BINARY_DIR}/bin)
set (CMAKE_LIBRARY_OUTPUT_DIRECTORY ${PROJECT_BINARY_DIR}/lib)
set (CMAKE_ARCHIVE_OUTPUT_DIRECTORY ${PROJECT_BINARY_DIR}/lib)

add_subdirectory(src)
