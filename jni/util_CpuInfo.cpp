#include "util_CpuInfo.h"
#include <iostream>
#include <cstring>

//https://www.cnblogs.com/xiaojianliu/articles/8735312.html
static void getCpuId(char* cpuid) {
    unsigned int deax, debx, decx, dedx, fn_id = 1;

    if (cpuid == nullptr) {
        return;
    }

    __asm__ ("cpuid"
			 :"=a"(deax),
			 "=b"(debx),
			 "=c"(decx),
			 "=d"(dedx)
			 :"a"(fn_id));
	
    // printf("id is %08X-%08X-%08X-%08X\n", deax, debx, decx, dedx);
    // id is dedx-deax

    snprintf(cpuid, 17, "%08x%08x", dedx, deax);
}


JNIEXPORT jstring JNICALL Java_util_CpuInfo_asmGetCpuId(JNIEnv *env, jclass) {
    printf("start jni\n");
    char cpuid[17] = {0};
    getCpuId(cpuid);

    //定义java String类 strClass
	jclass strClass = (env)->FindClass("Ljava/lang/String;");
	//获取String(byte[],String)的构造器,用于将本地byte[]数组转换为一个新String
	jmethodID ctorID = (env)->GetMethodID(strClass, "<init>", "([BLjava/lang/String;)V");
	//建立byte数组
	jbyteArray bytes = (env)->NewByteArray(strlen(cpuid));
	//将char* 转换为byte数组
	(env)->SetByteArrayRegion(bytes, 0, strlen(cpuid), (jbyte*) cpuid);
	// 设置String, 保存语言类型,用于byte数组转换至String时的参数
	jstring encoding = (env)->NewStringUTF("UTF-8");
	//将byte数组转换为java String,并输出
	return (jstring) (env)->NewObject(strClass, ctorID, bytes, encoding);
}