#ifndef CURSOR_H
#define CURSOR_H


class Cursor
{
public:
    Cursor(int x, int y, bool l) { _xCord = x; _yCord = y; _isActive = l; }
    int xCord() { return _xCord; }
    int yCord() { return _yCord; }
    bool isActive() { return _isActive; }
    void setXCord(int x) { _xCord = x; }
    void setYCord(int x) { _yCord = x; }
    void setActive(bool l) { _isActive = l; }
private:
    int _xCord;
    int _yCord;
    bool _isActive;
};

#endif // CURSOR_H
