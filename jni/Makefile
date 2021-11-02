cc = gcc
src = util_CpuInfo.cpp
JNI_PATH = /usr/lib/jvm/java-11-openjdk-amd64/include/
JNI_LINUX_PATH = /usr/lib/jvm/java-11-openjdk-amd64/include/linux
dist = ../../lib/libcpuid.so

all:
	$(cc) $(src) -I $(JNI_PATH) -I $(JNI_LINUX_PATH) -o -fPIC -shared -o $(dist)