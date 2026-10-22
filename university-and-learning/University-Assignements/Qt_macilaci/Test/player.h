#ifndef PLAYER_H
#define PLAYER_H


class Player
{

public:
    Player();
    Player( int x, int y );
    Player( int x, int y, int dX, int dY );
    int prevPosX;
    int prevPosY;
    int posX;
    int posY;
    int dirX;
    int dirY;
    int getNextX() { return posX + dirX; }
    int getNextX( int x ) { return posX + x; }
    int getNextY() { return posY + dirY; }
    int getNextY( int y ) { return posY + y; }

    void changeDirection();
    void step();
    void step( int dX, int dY );

};

#endif // PLAYER_H
