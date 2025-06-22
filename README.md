# CBL_ROS2_ROBORAMP

## Instructions on how to run the project:

Make sure required packages have been installed (API connection package, Goal Feasibility package, Battery status mock package, although these are not mandatory for the project to run and be tested).


---

### Getting started with the robot, connecting to the physical twin:

- Turn on the robot, open a terminal on the lab laptop, and connect to the robot using `ssh ubuntu@<the IP on the robot>`. The password is on top of the robot as well. You may be promted to type in "yes" the first time.
- In the terminal enter `export TURTLEBOT3_MODEL=burger`.
- Enter `ros2 launch turtlebot3_bringup robot.launch.py`.

---

### Starting the navigation:

Make sure teleop keyboard is not running at the same time as navigation.

Make sure you gave the robot the pos estimate before running navigation.

If navigation is not running properly, turn off cartographer. That part of the project can be fuly tested at home (virtual obstacles)

- `ros2 launch turtlebot3_cartographer [cartographer.launch.py](http://cartographer.launch.py/) use_sim_time:=True`
- `ros2 run nav2_map_server map_saver_cli -f ~/map`
- `ros2 launch turtlebot3_navigation2 [navigation2.launch.py](http://navigation2.launch.py/) use_sim_time:=True map:=$HOME/map.yaml`
- `ros2 run ros_tcp_endpoint default_server_endpoint --ros-args -p ROS_IP:=`


---

Run the unity TCP connection and start the project. 

