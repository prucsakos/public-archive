#include "player.h"

Player::Player()
{
 posX = 0;
 posY = 0;
}

Player::Player( int x, int y)
{
 posX = x;
 posY = y;
}

Player::Player( int x, int y, int dX, int dY)
{
 dirX = dX;
 dirY = dY;
 posX = x;
 posY = y;
}

void Player::changeDirection()
{
    dirX = dirX * (-1);
    dirY = dirY * (-1);
}

void Player::step()
{
    prevPosX = posX;
    prevPosY = posY;
    posX = posX + dirX;
    posY = posY + dirY;
}

void Player::step( int dX, int dY )
{
    prevPosX = posX;
    prevPosY = posY;
    posX = posX + dX;
    posY = posY + dY;
}
