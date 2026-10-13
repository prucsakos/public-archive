#include "rubikt.h"

#include <QApplication>

int main(int argc, char *argv[])
{
    QApplication a(argc, argv);
    RubikT w;
    w.show();
    return a.exec();
}
