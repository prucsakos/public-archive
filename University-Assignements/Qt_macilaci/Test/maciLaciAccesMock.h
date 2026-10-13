#ifndef MACILACIACCESMOCK_H
#define MACILACIACCESMOCK_H

#include <QDebug>
#include <macilaciacces.h>

class maciLaciAccesMock : public macilaciacces
{
public:
    void loadGame(int gameIndex, int &bagCount, QVector<Player*> &guards, int &mapWidth, int &mapHeight, QVector<QVector<int>> &map, QTimer &timer){
        bagCount = 0;
        mapWidth = 3;
        mapHeight = 3;
        map.resize(mapHeight);
        for(int i=0; i < mapHeight; i++){
            map[i].resize(mapWidth);                // minden érték 0
        }

        qDebug() << "game loaded to slot (" << gameIndex << ") with values: ";
        qDebug() << endl;
        for(int i=0; i< mapHeight; i++ ){
            for(int j = 0; j < mapWidth; j++){
                qDebug() << map[i][j] << " ";
            }
            qDebug() << endl;
        }

    }

signals:

};

#endif // MACILACIACCESMOCK_H
