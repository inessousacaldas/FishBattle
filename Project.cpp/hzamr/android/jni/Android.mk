LOCAL_PATH := $(call my-dir)

include $(CLEAR_VARS)
LOCAL_FORCE_STATIC_EXECUTABLE := true
LOCAL_MODULE := hzamr
LOCAL_C_INCLUDES := $(LOCAL_PATH)/../../hzamr

LOCAL_CPPFLAGS := -O3 -ffast-math
LOCAL_SRC_FILES :=  ../../hzamr/interf_dec.c \
		    ../../hzamr/interf_enc.c \
		    ../../hzamr/Plugin.cpp \
		    ../../hzamr/sp_dec.c \
		    ../../hzamr/sp_enc.c \
		    ../../hzamr/wavwriter.c \

include $(BUILD_SHARED_LIBRARY)