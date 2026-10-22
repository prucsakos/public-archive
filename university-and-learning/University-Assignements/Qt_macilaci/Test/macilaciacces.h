#ifndef MACILACIACCES_H
#define MACILACIACCES_H

#include <QObject>
#include <QTimer>
#include <QVector>
#include <player.h>
#include <QFile>
#include <QTextStream>
#include <QStringList>

class macilaciacces : public QObject
{
    Q_OBJECT
public:
    explicit macilaciacces();
    void loadGame(int gameIndex, int &bagCount, QVector<Player*> &guards, int &mapWidth, int &mapHeight, QVector<QVector<int>> &map, QTimer &timer);

signals:

};

#endif // MACILACIACCES_H
