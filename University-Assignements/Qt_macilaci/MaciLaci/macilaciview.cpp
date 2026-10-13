#include "macilaciview.h"

MaciLaciView::MaciLaciView(QWidget *parent)
    : QWidget(parent)
{
    //stylesheet
    FreeStepStyle = "background-color: White;";
    WallStyle = "background-color: Black;";
    BagStyle = "background-color: Green;";
    GuardStyle = "background-color: Red;";
    PlayerStyle = "background-color: Blue;";


    setFixedSize(400, 200);

    active = true;

    stopwatch = new QTimer(this);
    stopwatch->setInterval(1000);


    helpMB = new QMessageBox(this);
    endMB = new QMessageBox(this);

    mainLayout = new QVBoxLayout(this);
    menuLayout = new QHBoxLayout();
    outLayout = new QHBoxLayout();
    gameGrid = new QGridLayout();
    gameGrid->setHorizontalSpacing(0);
    gameGrid->setVerticalSpacing(0);
    gameGrid->setSpacing(0);
    gameGrid->setContentsMargins(0,0,0,0);

    menuV1 = new QVBoxLayout();
    menuV2 = new QVBoxLayout();

    newGameButton = new QPushButton("New Game");
    selectMapLabel = new QLabel("Select level");
    selectMapSpin = new QSpinBox();
    selectMapSpin->setMaximum(3);
    selectMapSpin->setMinimum(1);
    pauseButton = new QPushButton("Pause");
    helpButton = new QPushButton("Help");

    time = new QLabel("Timer: ");
    bagCount = new QLabel("Bags left: ");

    menuV1->addWidget(selectMapLabel);
    menuV1->addWidget(selectMapSpin);
    menuV1->addWidget(newGameButton);

    menuV2->addWidget(helpButton);
    menuV2->addWidget(pauseButton);

    menuLayout->addLayout(menuV1);
    menuLayout->addLayout(menuV2);

    outLayout->addWidget(time);
    outLayout->addWidget(bagCount);

    mainLayout->addLayout(menuLayout);
    mainLayout->addLayout(outLayout);
    mainLayout->addLayout(gameGrid);

    setLayout(mainLayout);

    _modell = new macilacimodell();

    connect(newGameButton, SIGNAL(clicked()), this, SLOT(newGame()));
    connect(helpButton, SIGNAL(clicked()), this, SLOT(helpButtonClicked()));
    connect(pauseButton, SIGNAL(clicked()), this, SLOT(pauseButtonClicked()));
    connect(&(_modell->timer), SIGNAL(timeout()), this, SLOT(stepGuards()));
    connect(stopwatch, SIGNAL(timeout()), this, SLOT(secPassed()));



}

MaciLaciView::~MaciLaciView()
{
}

void MaciLaciView::newGame()
{
    _modell->loadGame( selectMapSpin->value() );   // a modellbe betöltöm a pályát
    for( QVector<QPushButton*> buttons : buttonGrid )  // resetelem a buttongridet.
    {
        for(QPushButton* button : buttons){
            delete button;
        }
        buttons.clear();
    }
    buttonGrid.clear();
    buttonGrid.resize(_modell->mapHeight);
    for(int i = 0; i < _modell->mapHeight; i++){
        buttonGrid[i].resize(_modell->mapWidth);
    }

    for(int i=0; i < _modell->mapHeight; i++){
        for(int j=0; j < _modell->mapWidth; j++){
            QPushButton* button = new QPushButton("",this);
            button->setFixedSize(60,60);
            buttonGrid[i][j] = button;
            gameGrid->addWidget(buttonGrid[i][j],i,j);
        }
    }

    setWindowSize();
    drawMap();
    drawPlayer();
    drawGuards();
    secondsPassed = 0;
    time->setText("Timer: " + QString::number(secondsPassed));
    updateBagCount();
    stopwatch->start();
    if(pauseButton->text() == "Continue") pauseButtonClicked();
    setFocus();
    //this->close();
}

void MaciLaciView::setWindowSize()
{
    int height = 200 + ( _modell->mapHeight * 60 );
    int width = 400;
    if ( (_modell->mapWidth * 60) > 400 ) width = _modell->mapWidth * 60;
    setFixedSize(width,height);


}
void MaciLaciView::helpButtonClicked()
{
    helpMB->setText("Your name is Laci! Collect all the bags before the guards kill you!!!\n"
                    "You are the blue square.\n"
                    "Bags are green.\n"
                    "Guards are red.\n"
                    "Select a level, then press 'New Game' to start.\n"
                    "Use the arrows on your keyboard to move.");
    helpMB->show();
    setFocus();
}

void MaciLaciView::pauseButtonClicked()
{
    if(pauseButton->text() == "Pause")
        {
            _modell->timer.stop();
            stopwatch->stop();
            active = false;
        pauseButton->setText("Continue");
    } else {
        _modell->timer.start();
        stopwatch->start();
        active = true;
        pauseButton->setText("Pause");
    }
    setFocus();
}

void MaciLaciView::drawMap()
{
    for(int i = 0; i< _modell->mapHeight; i++){
        for(int j = 0; j < _modell->mapWidth; j++){
            int color = _modell->map[i][j];
            if(color == 0){
                buttonGrid[i][j]->setStyleSheet(FreeStepStyle);
            } else if(color == 1){
                buttonGrid[i][j]->setStyleSheet(WallStyle);
            } else {
                buttonGrid[i][j]->setStyleSheet(BagStyle);
            }
        }
    }

}


void MaciLaciView::drawPlayer()
{
    int x = _modell->player.posX;
    int y = _modell->player.posY;
    buttonGrid[x][y]->setStyleSheet(PlayerStyle);
}

void MaciLaciView::erasePlayer()
{
    int x = _modell->player.prevPosX;
    int y = _modell->player.prevPosY;
    buttonGrid[x][y]->setStyleSheet(getMapColor(x,y));
}

void MaciLaciView::drawGuards()
{
    for(Player* guard : _modell->guards)
    {
        int x = guard->posX;
        int y = guard->posY;
        buttonGrid[x][y]->setStyleSheet(GuardStyle);
    }
}

void MaciLaciView::eraseGuards()
{
    for(Player* guard : _modell->guards)
    {
        int x = guard->prevPosX;
        int y = guard->prevPosY;
        buttonGrid[x][y]->setStyleSheet(getMapColor(x,y));
    }
}

QString MaciLaciView::getMapColor( int x, int y)
{
    int color = _modell->map[x][y];
    if(color == 0){
        return FreeStepStyle;
    } else if(color == 1){
        return WallStyle;
    } else {
        return BagStyle;
    }
}

void MaciLaciView::stepGuards(){
    eraseGuards();
    drawGuards();
    if( _modell->isOver() ) gameOver();

}


void MaciLaciView::keyPressEvent(QKeyEvent *event)
{
    if(active){
        switch (event->key()) {
        case Qt::Key_Up:
            _modell->stepPlayer(-1, 0);
            break;
        case Qt::Key_Down:
            _modell->stepPlayer(1, 0);
            break;
        case Qt::Key_Left:
            _modell->stepPlayer(0, -1);
            break;
        case Qt::Key_Right:
            _modell->stepPlayer(0, 1);
            break;
            }
        updateBagCount();
        erasePlayer();
        drawPlayer();
        if( _modell->isOver() )
            {
                gameOver();
        } else if( _modell->isEnd() ) {
                gameWin();
        }
    }
}

void MaciLaciView::gameOver()
{
    newGame();
}

void MaciLaciView::gameWin()
{
    endMB->setText("You have completed this level! Time: " + QString::number(secondsPassed) + " seconds." );
    endMB->show();
    newGame();
}

void MaciLaciView::updateBagCount()
{
    bagCount->setText("Bags left: " + QString::number(_modell->bagCount) );
}

void MaciLaciView::secPassed()
{
    secondsPassed += 1;
    time->setText("Timer: " + QString::number(secondsPassed));
}

