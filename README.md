# CBL_ROS2_ROBORAMP

## Command sequence, all in separate terminal windows:
- ros2 launch turtlebot3_gazebo turtlebot3_world.launch.py
- ros2 launch turtlebot3_cartographer cartographer.launch.py use_sim_time:=True
- ros2 launch turtlebot3_navigation2 navigation2.launch.py use_sim_time:=True map:=$HOME/<name.of.map>.yaml
- ros2 run ros_tcp_endpoint default_server_endpoint --ros-args -p ROS_IP:= <laptop.IP or docker.IP> 

## External packages:
- Behavior tree monitor: https://github.com/Davidd0605/behavior_tree_monitor
- Ramp request API: https://github.com/szutciek/cbl_ros2_api_package
  
