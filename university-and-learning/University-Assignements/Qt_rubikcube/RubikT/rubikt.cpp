#include "rubikt.h"

RubikT::RubikT(QWidget *parent)
    : QWidget(parent)
{
    setFixedSize(450,500);
    setFocusPolicy(Qt::FocusPolicy::ClickFocus);

    //STYLESHEET
    pushButtonStyleUnSelected = "border-style: solid;" "border-width: 3px;" "border-color: white;";
    pushButtonStyleFrozen = "border-style: solid; " "border-width: 8px;" "border-color: purple;";



    rubikM = new RubikModell();
    cursor = new Cursor(0,0,true);

    overMB = new QMessageBox(this);
    helpMB = new QMessageBox(this);

    vert = new QVBoxLayout();
    hor = new QHBoxLayout();
    nGameButton = new QPushButton("Új játék", this);
    helpButton = new QPushButton("Segítség", this);
    stepsLabel = new QLabel("Lépések száma",this);
    hor->addWidget(nGameButton);
    hor->addWidget(helpButton);
    vert->addLayout(hor);
    vert->addWidget(stepsLabel);
    grid = new QGridLayout();
    vert->addLayout(grid);
    setLayout(vert);

    buttonGrid.resize(4);
    for(int i=0; i<4; i++){
        buttonGrid[i].resize(4);
        for(int j=0; j<4; j++){
            QPushButton* button = new QPushButton("",this);
            button->setFixedSize(100,100);
            buttonGrid[i][j] = button;
            grid->addWidget(buttonGrid[i][j],i,j);
        }
    }

    grid->setHorizontalSpacing(0);
    grid->setVerticalSpacing(5);


    connect(nGameButton, SIGNAL(clicked()), this, SLOT(nGameButtonPressed()));
    connect(helpButton, SIGNAL(clicked()), this, SLOT(helpButtonPressed()));

}

RubikT::~RubikT()
{
    delete rubikM;
    delete cursor;
    for (int i = 0; i<4 ; i++ ) {
        for (int j = 0 ; j<4 ; j++ ) {
            delete buttonGrid[i][j];
        }
    }
    delete overMB;
    delete helpMB;
    delete vert;
    delete hor;
    delete grid;
    delete nGameButton;
    delete helpButton;
    delete stepsLabel;
}

void RubikT::newGame(){
    rubikM->newGame();
    QVector<QVector<int>> colors = rubikM->getColors();
    RubikT::loadColors(colors);
    stepsLabel->setText("Lépések száma: " + QString::number(rubikM->getSteps()));
    setFocus();



}

QString RubikT::getColor(int x){
    QString result;
    switch (x) {
        case(0): result = "red" ;break;
        case(1): result = "blue" ;break;
        case(2): result = "green" ;break;
        case(3): result = "yellow" ;break;
    }
    return result;
}

void RubikT::loadColors(QVector<QVector<int>> colors){
    for(int i=0; i<4; i++){
        for(int j=0; j<4; j++){
            QString colorstring = getColor(colors[i][j]);
            buttonGrid[i][j]->setStyleSheet("background-color: " + colorstring + ";" +pushButtonStyleUnSelected);
            //if(i == cursor->xCord() || j == cursor->yCord()) buttonGrid[i][j]->setStyleSheet("background-color: " + colorstring + ";" +pushButtonStyleSelected);
            if(i == cursor->xCord() && j == cursor->yCord()) buttonGrid[i][j]->setStyleSheet("background-color: " + colorstring + ";" +"border-style: solid; " "border-width: 8px;" "border-color: black;");
            if((i == cursor->xCord() && j == cursor->yCord()) && !cursor->isActive()) buttonGrid[i][j]->setStyleSheet("background-color: " + colorstring + ";" +pushButtonStyleFrozen);
        }
    }
}


void RubikT::keyPressEvent(QKeyEvent *event){

        if(cursor->isActive()){
            switch (event->key()) {
            case Qt::Key_Up:
                RubikT::moveCursor(-1,0);
                break;
            case Qt::Key_Down:
                RubikT::moveCursor(1,0);
                break;
            case Qt::Key_Left:
                RubikT::moveCursor(0,-1);
                break;
            case Qt::Key_Right:
                RubikT::moveCursor(0,1);
                break;
            case Qt::Key_Space:
                frozeCursor();
                break;

                }
        }
        else {
            switch (event->key()) {
            case Qt::Key_Up:
                rubikM->slideUp(cursor->yCord());
                RubikT::loadColors(rubikM->getColors());
                stepsLabel->setText("Lépések száma: " + QString::number(rubikM->getSteps()));
                break;
            case Qt::Key_Down:
                rubikM->slideDown(cursor->yCord());
                RubikT::loadColors(rubikM->getColors());
                stepsLabel->setText("Lépések száma: " + QString::number(rubikM->getSteps()));
                break;
            case Qt::Key_Left:
                rubikM->slideLeft(cursor->xCord());
                RubikT::loadColors(rubikM->getColors());
                stepsLabel->setText("Lépések száma: " + QString::number(rubikM->getSteps()));
                break;
            case Qt::Key_Right:
                rubikM->slideRight(cursor->xCord());
                RubikT::loadColors(rubikM->getColors());
                stepsLabel->setText("Lépések száma: " + QString::number(rubikM->getSteps()));
                break;
            case Qt::Key_Space:
                frozeCursor();
                break;
                }
            if(rubikM->isEnd()) gameOver();
        }

}

void RubikT::frozeCursor(){
    if(cursor->isActive()) {
       cursor->setActive(false);
       for(int i=0;i<4;i++){
           for(int j=0;j<4;j++){
               if(i == cursor->xCord() && j == cursor->yCord()){
                   buttonGrid[i][j]->setStyleSheet("background-color: " + getColor(rubikM->getColorsItem(i,j)) +"; " + RubikT::pushButtonStyleFrozen);
               }
           }
       }
    } else {
        cursor->setActive(true);
        for(int i=0;i<4;i++){
            for(int j=0;j<4;j++){
                if(i == cursor->xCord() && j == cursor->yCord()) buttonGrid[i][j]->setStyleSheet("background-color: " + getColor(rubikM->getColorsItem(i,j)) + ";" +"border-style: solid; " "border-width: 8px;" "border-color: black;");
//                if(i == cursor->xCord() || j == cursor->yCord()){
//                    buttonGrid[i][j]->setStyleSheet("background-color: " + getColor(rubikM->getColorsItem(i,j)) +"; " + RubikT::pushButtonStyleSelected);
//                }
            }
        }
    }

}

void drawCursor(){

}

void RubikT::moveCursor(int x, int y){
    int newX = (cursor->xCord() + x) % 4;
    int newY = (cursor->yCord() + y) % 4;
    if(newX < 0) newX = 3;
    if(newY < 0) newY = 3;
    for(int i=0;i<4;i++){
        for(int j=0;j<4;j++){
            if(i == cursor->xCord() && j == cursor->yCord()) buttonGrid[i][j]->setStyleSheet("background-color: " + getColor(rubikM->getColorsItem(i,j)) +"; " + RubikT::pushButtonStyleUnSelected);

        }
    }
    cursor->setXCord(newX);
    cursor->setYCord(newY);

    for(int i=0;i<4;i++){
        for(int j=0;j<4;j++){
            if(i == cursor->xCord() && j == cursor->yCord()) buttonGrid[i][j]->setStyleSheet("background-color: " + getColor(rubikM->getColorsItem(i,j)) + ";" +"border-style: solid; " "border-width: 8px;" "border-color: black;");
//            if(i == cursor->xCord() || j == cursor->yCord() ){
//                buttonGrid[i][j]->setStyleSheet("background-color: " + getColor(rubikM->getColorsItem(i,j)) +"; " + RubikT::pushButtonStyleSelected);
//            }
        }
    }

}

void RubikT::nGameButtonPressed() {
    newGame();
}

void RubikT::helpButtonPressed(){
    helpMB->setText("A feladat, hogy alkoss csíkokat (sor vagy oszlop) a színekből. Használd a nyilakat a kurzor mozgatásához. A SPACE-el lefagyaszthatod a kurzort, és mozgathatod a sorokat/oszlopokat.");
    helpMB->show();
    this->setFocus();
}

void RubikT::gameOver(){
    overMB->setText(QString::number(rubikM->getSteps()) + " lépésből sikerült kiraknod!");
    overMB->show();
    newGame();

}
