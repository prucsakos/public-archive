#include "macilacimodell.h"

macilacimodell::macilacimodell()
{
    connect(&timer, SIGNAL(timeout()), this, SLOT(guardSteps()));
    mapHeight = 0;
    mapWidth = 0;

}

void macilacimodell::loadGame( int gameIndex )
{
    timer.stop();
    guards.clear();
    player.prevPosX = 0;
    player.prevPosY = 0;
    player.posX = 0;
    player.posY = 0;
    acces.loadGame( gameIndex, bagCount, guards, mapWidth, mapHeight, map, timer);
    timer.start();

}

void macilacimodell::guardSteps()
{
    for( Player *guard : guards){
        int x = guard->getNextX();
        int y = guard->getNextY();
        if( x >= 0 && x < (mapHeight) && y >= 0 && y < mapWidth ) {
            int k = map[x][y];
            if( (k == 0 || k == 2) ){
                guard->step();
            } else {
                guard->changeDirection();
            }
        } else {
            guard->changeDirection();
        }

    }
}
void macilacimodell::stepPlayer(int x, int y)
{
    int newX = player.getNextX( x );
    int newY = player.getNextY( y );
    if( newX >= 0 && newX < mapHeight && newY >= 0 && newY < mapWidth ) {
        int k = map[newX][newY];
        if( k == 0 ) {
            player.step( x, y );
        } else if( k == 2 ) {
            player.step( x, y );
            bagCount = bagCount - 1;
            map[newX][newY] = 0;

        }
    }
}

bool macilacimodell::isEnd()
{
    return bagCount == 0;
}

bool macilacimodell::isOver()
{
    for(Player* guard : guards)
        {
        int gX = guard->posX;
        int gY = guard->posY;
        int pX = player.posX;
        int pY = player.posY;
        int distX = gX - pX;
        int distY = gY - pY;
        if( (qFabs(distX) <= 1) && (qFabs(distY) <= 1) ) return true;
    }
    return false;
}

