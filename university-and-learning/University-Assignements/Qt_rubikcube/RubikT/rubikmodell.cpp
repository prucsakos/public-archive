#include "rubikmodell.h"
#include <QVector>
#include <QTime>
#include <QRandomGenerator>
RubikModell::RubikModell()
{
    colors.resize(4);
    for (int i=0;i<4;i++){
        colors[i].resize(4);
    }

}

void RubikModell::slideRight(int x){
    int k = colors[x][3];
    for(int y=3; y>0; y--){
        colors[x][y] = colors[x][y-1];
    }
    colors[x][0] = k;
    steps++;
}

void RubikModell::slideLeft(int x){
    int k = colors[x][0];
    for(int y=0; y<3; y++){
        colors[x][y] = colors[x][y+1];
    }
    colors[x][3] = k;
    steps++;
}

void RubikModell::slideUp(int y){
    int k = colors[0][y];
    for(int x=0; x<3; x++){
        colors[x][y] = colors[x+1][y];
    }
    colors[3][y] = k;
    steps++;
}

void RubikModell::slideDown(int y){
    int k = colors[3][y];
    for(int x=3; x>0; x--){
        colors[x][y] = colors[x-1][y];
    }
    colors[0][y] = k;
    steps++;
}

void RubikModell::newGame(){
    QRandomGenerator rg(QTime::currentTime().msec());
    QVector<int> s;
    for(int i=0;i<4;i++){
        s.push_back(0);
        s.push_back(1);
        s.push_back(2);
        s.push_back(3);
    }
    int indx;
    int k;
    for(int i=0;i<16;i++){
        indx = rg.bounded(0,15);
        k = s[i]; s[i] = s[indx]; s[indx] = k;

    }
    int x=0;
    for(int i=0; i<4; i++){
        for(int j=0;j<4;j++){
            colors[i][j] = s[x];
            x++;
        }
    }
    steps = 0;
}

bool RubikModell::isEnd() {
    bool result = true;
    int x = 0;
    int y;
    int color;
    while(result && x<4){
        color = colors[x][0];
        y = 1;
        while(result && y<4){
            result = result && (color == colors[x][y]);
            y++;
        }
        x++;
    }
    if(result) return true;
    result = true;
    y = 0;
    while(result && y<4){
        color = colors[0][y];
        x = 1;
        while(result && x<4){
            result = result && (color == colors[x][y]);
            x++;
        }
        y++;
    }
    if(result) return true;
    return false;
}

