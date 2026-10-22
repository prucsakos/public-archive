#ifndef RUBIKT_H
#define RUBIKT_H

#include <QWidget>
#include "rubikmodell.h"
#include "cursor.h"
#include <QVBoxLayout>
#include <QHBoxLayout>
#include <QPushButton>
#include <QLabel>
#include <QGridLayout>
#include <QKeyEvent>
#include <QMessageBox>
class RubikT : public QWidget
{
    Q_OBJECT

public:
    RubikT(QWidget *parent = nullptr);
    ~RubikT();
    void keyPressEvent(QKeyEvent *event);
private slots:
   void nGameButtonPressed();
   void helpButtonPressed();
private:
    void newGame();
    void frozeCursor();
    void moveCursor(int x, int y);
    void gameOver();
    QString getColor(int x);
    void loadColors(QVector<QVector<int>> colors);
    RubikModell *rubikM;
    Cursor *cursor;

    QVBoxLayout *vert;
    QHBoxLayout *hor;
    QPushButton *nGameButton;
    QPushButton *helpButton;
    QLabel *stepsLabel;
    QMessageBox *helpMB;
    QMessageBox *overMB;

    QGridLayout *grid;
    QVector<QVector<QPushButton*>> buttonGrid;

    QString pushButtonStyleSelected;
    QString pushButtonStyleUnSelected;
    QString pushButtonStyleFrozen;
};
#endif // RUBIKT_H
