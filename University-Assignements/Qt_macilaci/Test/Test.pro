QT += testlib
QT -= gui

CONFIG += qt console warn_on depend_includepath testcase
CONFIG -= app_bundle

TEMPLATE = app

SOURCES +=  tst_macilacimodelltest.cpp \
            macilacimodell.cpp \
            player.cpp  \
            macilaciacces.cpp


HEADERS +=  macilaciacces.h \
            maciLaciAccesMock.h \
            macilacimodell.h \
            player.h
