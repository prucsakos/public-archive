#include "macilaciview.h"

#include <QApplication>

int main(int argc, char *argv[])
{
    QApplication a(argc, argv);
    MaciLaciView w;
    w.show();
    return a.exec();
}
