#ifndef RUBIKMODELL_H
#define RUBIKMODELL_H

#include <QVector>
#include <QMainWindow>
class RubikModell
{
public:
    RubikModell();
    void slideRight(int y);
    void slideLeft(int y);
    void slideUp(int x);
    void slideDown(int x);
    void newGame();
    QVector<QVector<int>> getColors() {return colors;}
    int getColorsItem(int x, int y) { return colors[x][y]; }
    int getSteps() {return steps;}
    void step() { steps++; }
    bool isEnd();
private slots:

private:
    QVector<QVector<int>> colors;
    int steps;

};

#endif // RUBIKMODELL_H
