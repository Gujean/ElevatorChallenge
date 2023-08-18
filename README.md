# ElevatorChallenge

How to operate:
  eg: [Current Floor: 5, Destination Floor: 2, And the direction DOWN], i wrote it this way because i wanted to simulate
  someone requesting an elevator and then when the elevator arives from inside the elevator to select the destination

  and i wanted to create  a system that runs the elevators in paralel 
  
  just give commands: 5,2,arrow DOWN
                      8,3,arrow DOWN
                      1,7,arrow UP
                      5,9,arrow UP
                      
!!!! I didn't treat edge cases like:
  -other keys pressed than numbers or arrow UP or DOWN when is needed 
  -you give wrong commands eg: 5,2, arrow up(when i am at )
  -i didn't create a building class and then to be able to configure floors and who has access to those floors(that would have been easy)

!!! I did't create many tests
  -honestly you need to work on this for a while and it's not even for something important like some reallife situation 

!!!The code definetly needs some more refactoring and some more optimization on choosing the best elevator, but for how much time i got this will do for the moment

!!!i tried to implement only the most important things:
  -multiple floors
  -multiple elevators
  -optimized elevator picker by the requestedFloor and The destination more then the LOOK algorithm
  -elevators to run in parallel with parallel programming 
