/**
* Source Code Created by: Peter Howley
* Date 12/18/2020
* Time: ~4.5 hours
* References: Stack Overflow and GeekForGeeks mainly for translation purposes from c# to c++ and syntax questions.
*
* Bonus Details:
* Basic game of tic tac toe with the ability to play an m,n,k variation of the game at runtime or opt for the classic version.
* Allows the ability for users to undo their last move until a draw or winner is declared.
*
**/
#include <iostream>
#include <stack>

int playerVal = 0;
int count = 0;
std::stack<int> moves;

/// <summary>
/// Function to draw the board
/// </summary>
/// <param name="board"> 2D Array you wish to draw</param>
/// <param name="rows"> The amount of rows in the 2D array</param>
/// <param name="cols"> The amount of columns in the 2D array</param>
void WriteBoard(int** board, int rows, int cols)
{
    for (int i = 0; i < rows; i++)
    {
        for (int j = 0; j < cols; j++)
        {
            int value = board[i][j];
            //if we find a value of -2 we can convert it to an "O" and we write the " | " to make it look sort of nicer
            if (value == -2)
                std::cout << 'O' << " |  ";
            //-1 converts to X
            else if (value == -1)
                std::cout << 'X' << " |  ";
            else
            {
                //we do this check to keep single digit values relatively in line with double digit values so the board looks nicer
                if (value < 10)
                    std::cout << value << " |  ";
                else
                    std::cout << value << " | ";
            }
        }
        std::cout << "\n";
        //this part is solely to make horizontal lines for the board so it is easier to read
        if (i != rows - 1)
        {
            for (int j = 0; j < cols; j++)
                std::cout << "_____";
        }
        std::cout << "\n";
    }
}
/// <summary>
/// Function to initialize the board with default values
/// </summary>
/// <param name="board"> 2D Array representing the board</param>
/// <param name="rows"> The amount of rows in the 2D array</param>
/// <param name="cols"> The amount of columns in the 2D array</param>
void InitializeBoard(int** board, int rows, int cols)
{
    //initialize our board with a simple count from 1 to m*n
    int count = 1;
    for (int i = 0; i < rows; i++)
    {
        for (int j = 0; j < cols; j++)
        {
            board[i][j] = count;
            count++;
        }
    }
}
/// <summary>
/// A helper function used to check the rows from the last placed piece in order to determine if that player won the game.
/// </summary>
/// <param name="board"> 2D Array representing the board</param>
/// <param name="originx">The x value of the last placed piece</param>
/// <param name="originy">The y value of the last placed pieced</param>
/// <param name="k">The amount of pieces in a row needed to win</param>
/// <param name="direction">The direction to check 0 = top, 1 = bot, 2 = left, 3 = right. 4 = up-left, 5 = bot-right, 6 = bot-left, 7 = top-right</param>
/// <param name="count">The amount of pieces in a row thus far</param>
/// <param name="rows"> The amount of rows in the 2D array</param>
/// <param name="cols"> The amount of columns in the 2D array</param>
/// <returns>The amount of pieces in a row that were found</returns>
int checkDirection(int** board, int originx, int originy, int k, int direction, int count, int rows, int cols)
{
    //check all directions to see if we have a connect of size k or bigger to declare a winner
    // 0 = top, 1 = bot, 2 = left, 3 = right. 4 = up-left, 5 = bot-right, 6 = bot-left, 7 = top-right
    int currentValue = board[originx][originy];
    int playerValue = currentValue;
    switch (direction)
    {
        //up
    case 0:
        while (playerValue == currentValue)
        {
            originx--;
            //breaks if out of bounds
            if (originx < 0)
                break;
            currentValue = board[originx][originy];
            //if we find a similar piece adjacent we can increment our count
            if (currentValue == playerValue)
                count++;
        }
        break;
        //down
    case 1:
        while (playerValue == currentValue)
        {
            originx++;
            if (originx >= rows)
                break;
            currentValue = board[originx][originy];
            if (currentValue == playerValue)
                count++;
        }
        break;
        //left
    case 2:
        while (playerValue == currentValue)
        {
            originy--;
            if (originy < 0)
                break;
            currentValue = board[originx][originy];
            if (currentValue == playerValue)
                count++;
        }
        break;
        //right
    case 3:
        while (playerValue == currentValue)
        {
            originy++;
            if (originy >= cols)
                break;
            currentValue = board[originx][originy];
            if (currentValue == playerValue)
                count++;
        }
        break;
        //upleft
    case 4:
        while (playerValue == currentValue)
        {
            originx--;
            originy--;
            if (originy < 0 || originx < 0)
                break;
            currentValue = board[originx][originy];
            if (currentValue == playerValue)
                count++;
        }
        break;
        //botright
    case 5:
        while (playerValue == currentValue)
        {
            originx++;
            originy++;
            if (originx >= rows || originy >= cols)
                break;
            currentValue = board[originx][originy];
            if (currentValue == playerValue)
                count++;
        }
        break;
        //downleft
    case 6:
        while (playerValue == currentValue)
        {
            originx--;
            originy++;
            if (originx < 0 || originy >= cols)
                break;
            currentValue = board[originx][originy];
            if (currentValue == playerValue)
                count++;
        }
        break;
        //upright
    case 7:
        while (playerValue == currentValue)
        {
            originx++;
            originy--;
            if (originx >= rows || originy < 0)
                break;
            currentValue = board[originx][originy];
            if (currentValue == playerValue)
                count++;
        }
        break;
    }
    return count;
}
/// <summary>
/// A function that keeps calls to check the rows to see if the player who played the last placed piece won the game
/// </summary>
/// <param name="board"> 2D Array representing the board</param>
/// <param name="originx">The x value of the last placed piece</param>
/// <param name="originy">The y value of the last placed pieced</param>
/// <param name="k">The amount of pieces in a row needed to win</param>
/// <param name="rows"> The amount of rows in the 2D array</param>
/// <param name="cols"> The amount of columns in the 2D array</param>
/// <returns>Whether or not the player won the game</returns>
bool CheckForWin(int** board, int originx, int originy, int k, int rows, int cols)
{
    bool didWin = false;
    int count;
    int directionCheck = 0;
    //check all 8 directions to see if we have a connect-k winner, if we have a winner we have an early out here.
    while (directionCheck < 8 && !didWin)
    {
        //resets count back to 1 after you check both directions horizontally, vertically and diagnoally
        if (directionCheck % 2 == 0)
            count = 1;
        count = checkDirection(board, originx, originy, k, directionCheck, count, rows, cols);
        //if we find a connect of size k or bigger we know we have a winner and can declare and return it.
        if (count >= k)
            didWin = true;
        directionCheck++;
    }
    return didWin;
}
/// <summary>
/// A function that will undo the last move a that was made in the game.
/// </summary>
/// <param name="board"> 2D Array representing the board</param>
/// <param name="cols"> The amount of columns in the 2D array</param>
void UndoMove(int** board, int cols)
{
    //handles issue with user trying to undo a move when non are made.
    if (moves.empty())
        std::cout << "There are no moves to undo...\n";
    else
    {
        //gets the last move made.
        int moveToUndo = moves.top();
        moves.pop();
        //finds location of last move and puts the original value back
        moveToUndo--;
        int x = moveToUndo / cols;
        int y = moveToUndo % cols;
        board[x][y] = moveToUndo + 1;
        //switch player turns because we undid their last move
        if (playerVal == -2)
            playerVal = -1;
        else
            playerVal = -2;
        //decrement board fill so we don't declare early draw
        count--;
    }
}
/// <summary>
/// A function that handles everything involved in a players turn from prompting them to setting a piece down in a user-declared location
/// </summary>
/// <param name="board"> 2D Array representing the board</param>
/// <param name="rows"> The amount of rows in the 2D array</param>
/// <param name="cols"> The amount of columns in the 2D array</param>
/// <param name="k">The amount of pieces in a row needed to win</param>
/// <returns>Returns if the player won the game</returns>
bool PlayerTurn(int** board, int rows, int cols, int k)
{
    //switching turns so the next player can go afterwards
    if (playerVal == -2)
        playerVal = -1;
    else
        playerVal = -2;
    int input;
    int x;
    int y;
    do {
        //asks the player for thier move
        std::cout << "Please type a value to place piece (1, 5, 12, etc) or type 0 to undo move...\n";
        std::cin >> input;
        //handles issues with users typing in non-integer values
        if (std::cin.fail())
        {
            std::cin.clear();
            std::cin.ignore();
            WriteBoard(board, rows, cols);
        }
        else
            break;
    } while (true);
    while (true)
    {
        //if user inputs 0 they will undo the last move that is put in.
        if (input == 0)
        {
            UndoMove(board, cols);
            WriteBoard(board, rows, cols);
        }
        else if (input > 0 && input <= rows * cols)
        {
            //pushes move into a stack in case a player wishes to undo their moves later
            moves.push(input);
            //converts a user declared integer into a row and col location for 2d array
            input--;
            x = input / cols;
            y = input % cols;
            int currentVal = board[x][y];
            //handles issues with players trying to override squares that are already taken
            if (currentVal != 0 && currentVal != -1)
            {
                board[x][y] = playerVal;
                break;
            }
        }
        //keep asking user to get valid input
        WriteBoard(board, rows, cols);
        std::cout << "Please enter a valid move shown above...\n";
        std::cin >> input;
    }
    //check to see if a user won after they place their piece
    return CheckForWin(board, x, y, k, rows, cols);
}

int main()
{
    //takes input to see if game is a regular or modified game
    std::string standard;
    std::cout << "Standard game?(y/n)...\n";
    std::cin >> standard;
    int rows = 3;
    int cols = 3;
    int k = 3;
    //figure out if user wants to play with special conditions or if they wish to play a standard game
    while (standard != "y" && standard != "n")
    {
        std::cout << "Please input y or n...\n";
        std::cout << "Standard game?(y/n)...\n";
        std::cin >> standard;
    }
    if (standard == "n")
    {
        //takes user input for m,n,k variation
        std::cout << "Please enter rows...\n";
        std::cin >> rows;
        std::cout << "Please enter columns...\n";
        std::cin >> cols;
        std::cout << "Please enter pieces to win...\n";
        std::cin >> k;
        while (k > rows && k > cols)
        {
            //handles unwinnable games, a user shouldn't experience a game that no party can win...
            std::cout << "There is no way to win please enter a smaller number for pieces to win...\n";
            std::cin >> k;
        }
    }
    //create board based on user parameters
    int** board = new int* [rows];
    //allocates space for 2d array
    for (int i = 0; i < rows; ++i)
    {
        board[i] = new int[cols];
    }
    //initializes board with values to place pieces
    InitializeBoard(board, rows, cols);
    //writes the base state of the board
    WriteBoard(board, rows, cols);
    //sets player turn to O
    int playerVal = -1;
    //represents if a player won
    bool didWin = false;
    //plays until board is full
    while (count < rows * cols)
    {
        //calls the turn of the current player
        didWin = PlayerTurn(board, rows, cols, k);
        //keeping track if board is full
        count++;
        WriteBoard(board, rows, cols);
        //does an early out if a winner is found and declares who won
        if (didWin)
        {
            if (count % 2 == 0)
                std::cout << "Player X Wins!\n";
            else
                std::cout << "Player O Wins!\n";
            break;
        }
    }
    //if no winner declares draw
    if (!didWin)
        std::cout << "Draw!\n";
}

