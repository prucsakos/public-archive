#ifndef MACILACIMODELL_H
#define MACILACIMODELL_H

#include <QObject>
#include <maciLaciAccesMock.h>
#include <QVector>
#include <QtMath>
#include "player.h"

class macilacimodell : public QObject
{
    Q_OBJECT
public:
    macilacimodell();
    maciLaciAccesMock acces;
    int bagCount;


    Player player;
    QVector<Player*> guards;

    int mapWidth;
    int mapHeight;
    QVector<QVector<int>> map;

    QTimer timer;
    void stepPlayer(int x, int y);
    bool isEnd();
    bool isOver();
    void loadGame( int gameIndex );


signals:
private slots:
    void guardSteps();

};

#endif // MACILACIMODELL_H
