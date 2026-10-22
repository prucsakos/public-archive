#include <QtTest>
#include <QCoreApplication>
#include "maciLaciAccesMock.h"
#include "macilacimodell.h"

// add necessary includes here

class MaciLaciModellTest : public QObject
{
    Q_OBJECT

public:

private:
    macilacimodell* _model;

private slots:
    void initTestCase();
    void cleanupTestCase();
    void testNewGame();
    void testStepGame();

};


void MaciLaciModellTest::initTestCase()
{
    _model = new macilacimodell();
}

void MaciLaciModellTest::cleanupTestCase()
{
    delete _model;
}

void MaciLaciModellTest::testNewGame()
{
    _model->loadGame(1);


    QCOMPARE(_model->bagCount, 0);
    for (int i = 0; i < _model->mapHeight; i++)
        for (int j = 0; j < _model->mapHeight; j++)
            QCOMPARE(_model->map[i][j], 0);
}

void MaciLaciModellTest::testStepGame()
{
    _model->loadGame(1);
    QCOMPARE(_model->player.posX,0);   // a 0,0 mezőn kezd a játékos
    QCOMPARE(_model->player.posY,0);

    _model->stepPlayer(-1,-1);  // nem tud lelépni a pályáról
    QCOMPARE(_model->player.posX,0);
    QCOMPARE(_model->player.posY,0);

    _model->stepPlayer(1,1);
    QCOMPARE(_model->player.posX,1); // a pályán szabadon közlekedhet.
    QCOMPARE(_model->player.posY,1);
}


QTEST_MAIN(MaciLaciModellTest)

#include "tst_macilacimodelltest.moc"
