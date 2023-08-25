import gymnasium as gym
from gymnasium import spaces
import torch as nn
import PIL as pil
from PIL import Image as pilimage
import torchvision.transforms as transforms
import numpy as np
import traceback
import keyboard
import socket
from datetime import datetime
import subprocess
from WindowUtility import get_window_rect
from os import path , makedirs
import mss
from mss.tools import to_png
from time import sleep

def run_game(game_process, game_log, game_exec_args):
    if game_process is not None:
        game_process.terminate()
    if game_log is not None:
        game_log.close()

    game_path = game_exec_args["game_path"]
    game_args = [game_path].append(game_exec_args["game_args"])
    log_path = game_exec_args["log_path"]
    game_process = subprocess.Popen(game_args, stdout=subprocess.PIPE)
    sleep(1)
    game_log = open(log_path, 'r')
    game_rect = get_window_rect(game_process.pid)
    if game_rect is None:
        sleep(1)
        game_rect = get_window_rect(game_process.pid)

    

    print("game_rect:", game_rect)
    game_monitor = {"left": game_rect[0], "top": game_rect[1], "width": game_rect[2] - game_rect[0], "height": game_rect[3]-game_rect[1]}
    return game_process, game_monitor, game_log

class UnityGameEnv(gym.Env):
    #frame shape must be in torch format of C*H*W
    def __init__(self, frame_shape, input_schema):
        self.frame_shape = frame_shape
        self.input_schema = input_schema
        self.observation_space = spaces.Box(low=0, high=255, shape=frame_shape, dtype=np.uint8)
        # input_spaces = [spaces.Discrete(2) if sc == 'B'
        #                 else spaces.Box(low=-1,high=1, dtype=np.float32) if sc =="A"
        #                 else spaces.Box(low=np.array([0,0]), high=np.array([frame_shape[1],frame_shape[2]]), dtype=np.uint16) if sc =="M"
        #                 else None
        #                 for sc in input_schema]
        # self.action_space = spaces.Sequence(input_spaces)
        input_spaces = []
        for sc in input_schema:
            if sc =="B":
                input_spaces.append(2)
            elif sc == "A":
                input_spaces.append(3)
            elif sc == "M":
                input_spaces.append(frame_shape[0])
                input_spaces.append(frame_shape[1])
        self.action_space = spaces.MultiDiscrete(input_spaces)



        self.capture = False
        self.game_process = None
        self.game_monitor = None
        self.game_log = None
        #self.pil_to_tensor = transforms.Compose([transforms.PILToTensor()])

        self.game_socket_address = None
        self.UDPServerSocket = None
        #TODO: Separate dumps for each reset
        if self.capture:
            folder_path = path.join("captures", datetime.now().strftime("%Y-%m-%d %H-%M-%S"))
            makedirs(folder_path, exist_ok=True)
            input_dump = open(path.join(folder_path, "input_dump.txt"), 'w')

        self.localIP     = "127.0.0.1"
        self.localPort   = 20001
        self.bufferSize  = 1024
        self.screen_capturer = mss.mss()


    def _get_obs(self):
        if self.obs_full["deprecated"]:
            self._get_obs_full()
        return self.obs_full["obs"]
    
    def _get_obs_full(self):
        self.obs_full["deprecated"] = False
        process_poll = self.game_process.poll()

        # if print_stdout:
        #     outputs = process.stdout.readlines()
        #     for output in outputs:
        #         print(output.strip())

        log_reward = 0
        while True:
            log_line = self.game_log.readline()
            if not log_line:
                break
            else:
                if len(log_line) > 2 and log_line[0:3] == "ai;":
                    log_event, log_arg = log_line.strip().split(';')[1:3]
                    if log_event == "starting_level":
                        print("Starting level:", log_arg)
                    elif log_event == "starting_menu":
                        print("Starting menu:", log_arg)
                    elif log_event == "reward_menu":
                        print("Rewarding AI for menu:", log_arg)
                        log_reward += int(log_arg)
                    elif log_event == "reward_game":
                        print("Rewarding AI for game:", log_arg)
                        log_reward += int(log_arg)
        self.obs_full["reward"] = log_reward
        
        if process_poll is None:
            try:
                bytesAddressPair = self.UDPServerSocket.recvfrom(self.bufferSize)
                # print_stdout = True

                message = bytesAddressPair[0].decode()
                self.game_socket_address = bytesAddressPair[1]
                frame_capture = self.screen_capturer.grab(self.game_monitor)
                #img = pilimage.frombuffer("RGB", frame_capture.size, frame_capture.bgra, "raw", "BGRX")
                #self.obs_full["obs"] = self.pil_to_tensor(img)
                if self.obs_full["obs"] is None:
                    self.UDPServerSocket.settimeout(2)
                self.obs_full["obs"] = np.asarray(frame_capture)
                #print("dims:", self.obs_full["obs"].shape)
                if self.capture:
                    input_dump.write("game:"+ message + ";input:"+msgFromServer +"\n")
                    frame_file = path.join(folder_path, message.replace(',', '_') + ".png")
                    to_png(frame_capture.rgb, frame_capture.size, level=screen_capturer.compression_level, output=frame_file)

            except TimeoutError:
                print("timed out.")
            except:
                print("other socket error.")

        else:
            self.obs_full["terminated"] = True
            print("Game process finished with code:", process_poll)
            # if RELAUNCH > 0:
            #     RELAUNCH -= 1
            #     game_process.terminate()
            #     game_process = None
            # else:
            #     looping = False
            if process_poll != 0:
                #crash
                pass

    # def reset(self):
    #     return self.reset(0)

    def reset(self, seed=0):
        if self.UDPServerSocket is not None:
            self.UDPServerSocket.close()
            self.UDPServerSocket = None
        self.UDPServerSocket = socket.socket(family=socket.AF_INET, type=socket.SOCK_DGRAM)
        self.UDPServerSocket.settimeout(10)
        self.UDPServerSocket.bind((self.localIP, self.localPort))
        self.game_process, self.game_monitor, self.game_log = run_game(self.game_process, self.game_log)
        self.obs_full = {"obs": None, "reward":0, "terminated":False, "truncated":False, "info":{}, "deprecated":True}
        self._get_obs_full()
        return self.obs_full["obs"], self.obs_full["info"]

    def step(self, action):
        self.obs_full["deprecated"] = True
        act_idx = 0
        action_message = ""
        for sc in self.input_schema:
            action_message+=','
            if sc == "B":
                action_message += str(action[act_idx])
            elif sc == "A":
                action_message += str(action[act_idx]-1)
            elif sc == "M":
                action_message += str(action[act_idx]) + "~"
                act_idx +=1
                action_message += str(action[act_idx])
            act_idx += 1
        bytes2send = str.encode(action_message[1:])
        try:
            self.UDPServerSocket.sendto(bytes2send, self.game_socket_address)
        except TimeoutError:
            print("timed out.")
        obs = self._get_obs()
        return obs, self.obs_full["reward"], self.obs_full["terminated"], self.obs_full["truncated"], self.obs_full["info"]


    def close(self):
        self.screen_capturer.close()
        if self.UDPServerSocket is not None:
            self.UDPServerSocket.close()
            self.UDPServerSocket = None
        if self.game_process is not None:
            self.game_process.terminate()
        if self.game_log is not None:
            self.game_log.close()
        