#include "macilaciacces.h"

macilaciacces::macilaciacces()
{

}

void macilaciacces:: loadGame(int gameIndex, int &bagCount, QVector<Player*> &guards, int &mapWidth, int &mapHeight, QVector<QVector<int>> &map, QTimer &timer)
{
    QFile file("../MaciLaci/map" + QString::number(gameIndex) + ".txt");
    file.open(QFile::ReadOnly);
    if(file.isOpen()){
        QTextStream stream(&file);
        int guardCount;
        mapWidth = stream.readLine().toInt();
        mapHeight = stream.readLine().toInt();
        bagCount = stream.readLine().toInt();
        guardCount = stream.readLine().toInt();
        guards.resize(guardCount);
        timer.setInterval(stream.readLine().toInt());
        QString tmp;
        map.resize(mapHeight);
        for(int i=0; i < mapHeight; i++){
            map[i].resize(mapWidth);
            tmp = stream.readLine();
            for(int j=0; j < mapWidth; j++){
                if( tmp.at(j) == "0" ){
                    map[i][j] = 0;
                } else if( tmp.at(j) == "1" ){
                    map[i][j] = 1;
                } else {
                    map[i][j] = 2;
                }
            }
        }
        QStringList qst;
        int par1;
        int par2;
        int par3;
        int par4;
        for(int i = 0; i < guardCount; i++){
            tmp = stream.readLine();
            qst = tmp.split(" ");
            par1 = (qst.at(0)).toInt();
            par2 = (qst.at(1)).toInt();
            par3 = (qst.at(2)).toInt();
            par4 = (qst.at(3)).toInt();
            guards[i] = (new Player(par1, par2, par3, par4));

        }
    }
    file.close();
}
