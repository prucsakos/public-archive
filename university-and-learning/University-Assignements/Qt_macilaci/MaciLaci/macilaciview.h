#ifndef MACILACIVIEW_H
#define MACILACIVIEW_H

#include <QWidget>
#include "macilacimodell.h"
#include <QHBoxLayout>
#include <QVBoxLayout>
#include <QGridLayout>
#include <QPushButton>
#include <QKeyEvent>
#include <QLabel>
#include <QMessageBox>
#include <QSpinBox>
#include <QKeyEvent>
#include <QMessageBox>

class MaciLaciView : public QWidget
{
    Q_OBJECT

public:
    MaciLaciView(QWidget *parent = nullptr);
    ~MaciLaciView();
    void keyPressEvent(QKeyEvent *event);
private slots:

    void newGame(  );
    void helpButtonClicked();
    void stepGuards();
    void pauseButtonClicked();
    void secPassed();
private:
    macilacimodell *_modell;
    QTimer *stopwatch;
    int secondsPassed;
    bool active;

    QMessageBox *helpMB;
    QMessageBox *endMB;

    QVBoxLayout *mainLayout;

    QHBoxLayout *menuLayout;
    QVBoxLayout *menuV1;
    QVBoxLayout *menuV2;
    QHBoxLayout *outLayout;
    QPushButton *newGameButton;
    QLabel *selectMapLabel;
    QSpinBox *selectMapSpin;

    QPushButton *pauseButton;
    QPushButton *helpButton;
    QLabel *time;
    QLabel *bagCount;


    QGridLayout *gameGrid;
    QVector<QVector<QPushButton*>> buttonGrid;

    QString FreeStepStyle;
    QString WallStyle;
    QString BagStyle;
    QString PlayerStyle;
    QString GuardStyle;

    void setWindowSize();
    void drawMap();
    void drawPlayer();
    void drawGuards();
    void erasePlayer();
    void eraseGuards();
    QString getMapColor(int x, int y);
    void gameOver();
    void gameWin();
    void updateBagCount();

};
#endif // MACILACIVIEW_H
