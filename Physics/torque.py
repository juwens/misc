# -*- coding: utf-8 -*-
"""
Created on Mon Sep 25 23:17:38 2023

@author: jens
"""

import matplotlib.pyplot as plt
import numpy as np
import math
from typing import Callable

plt.rcParams['figure.dpi'] = 300

max_kmph = 250

def plot_torque(
        wheel_circumference: float,
        gear_ratios: list[float],
        engine_rpm_to_torque: Callable[[float], float],
        diff_ratio: int,
        fmt: str,
        max_rpm: int):
    
    wheel_radius = wheel_circumference / 2 / math.pi
    to_mps = lambda kmph : 1.0 * kmph / 3.6
    kmph_to_engine_rpm = lambda kmph, gear_ratio : to_mps(kmph) / wheel_circumference * gear_ratio * diff_ratio * 60
    
    def kmph_to_wheel_torque(kmph: float, gear_ratios: list[float], gear: int):
        engine_rpm = kmph_to_engine_rpm(kmph, gear_ratios[gear])
        
        if (engine_rpm < 500 or engine_rpm > max_rpm):
            return None
        
        engine_torque = engine_rpm_to_torque(engine_rpm)
        wheel_torque = engine_torque * gear_ratios[gear] * diff_ratio
        
        if (gear > 0):
            prev_gear_engine_rpm = kmph_to_engine_rpm(kmph, gear_ratios[gear-1])
            prev_gear_engine_torque = engine_rpm_to_torque(prev_gear_engine_rpm)
            prev_gear_wheel_torque = prev_gear_engine_torque * gear_ratios[gear-1] * diff_ratio
            if (prev_gear_wheel_torque - wheel_torque > (0.1 * wheel_torque)):
                #print([gear+1, round(engine_rpm), round(engine_torque), round(prev_gear_engine_rpm), round(prev_gear_engine_torque)])
                return None
        
        if (gear < len(gear_ratios) - 1):
            next_gear_engine_rpm = kmph_to_engine_rpm(kmph, gear_ratios[gear+1])
            next_gear_engine_torque = engine_rpm_to_torque(next_gear_engine_rpm)
            next_gear_wheel_torque = next_gear_engine_torque * gear_ratios[gear+1] * diff_ratio
            #if (next_gear_wheel_torque > wheel_torque):
            if (next_gear_wheel_torque - wheel_torque > (0.1 * wheel_torque)):
                #print([gear+1, round(engine_rpm), round(engine_torque), round(next_gear_engine_rpm), round(next_gear_engine_torque)])
                return None
        
        wheel_torque = engine_torque * gear_ratios[gear] * diff_ratio
        #print([round(engine_rpm), round(engine_torque), round(wheel_torque)])
        return wheel_torque
    
    def kmph_to_acc_force(kmph: float, gear_ratios: list[float], gear: int):
        torque = kmph_to_wheel_torque(kmph, gear_ratios, gear)
        if (not torque):
            return None
        return torque / wheel_radius
    
    x = np.linspace(0, max_kmph, num=max_kmph) # km/h
    for i in range(0, len(gear_ratios)):
        gear_ratio = gear_ratios[i]
        y = [kmph_to_acc_force(speed, gear_ratios, i) for speed in x]
        #plt.plot(x, y, fmt, label=str.format("gear {0} ({1})", i + 1, gear_ratio))
        plt.plot(x, y, fmt)
    
# https://www.motor-talk.de/forum/getriebeuebersetzung-t349060.html & https://de.carspec.info/skoda-octavia-combi-2.0-fsi-2004    
# 2.0 FSI; max Nm 200
plot_torque(
    wheel_circumference= 1.982, 
    gear_ratios = [3.78, 2.27, 1.52, 1.19, 0.97, 0.82], 
    engine_rpm_to_torque = lambda x : -7.8125*10**-13 * x**4 + 1.12847*10**-8 * x**3 - 0.0000622396 * x**2 + 0.157837 * x + 43.75, 
    diff_ratio = 3.65,
    fmt = '-m',
    max_rpm=6800)

#Golf 5 TDI 77kW; max Nm 250
plot_torque(
    wheel_circumference= 1.982,
    # https://www.motor-talk.de/bilder/1-9-tdi-e-getriebekennbuchstabe-lange-uebersetzung-g82430831/cool5-i209731245.html
    # https://www.doppel-wobber.de/community/index.php?thread/17844-getriebe-%C3%BCbersetzung/
    gear_ratios = [3.778, 2.267, 1.524, 1.107, 0.875, 0.725],
    # https://www.angurten.de/index.php?m2_id=1153&_B=infosystem&modus=motoren&modell_id=1&modell=A3&motor_id=58
    # https://www.wolframalpha.com/ : "fit Quartic {{1000, 150}, {1950, 250}, {3000, 220}, {4000, 175}, {4400, 120}}"
    engine_rpm_to_torque = lambda x : -1.27445e-11 * x**4 + 1.46442e-7 * x**3 - 0.000624703 * x**2 + 1.13984 * x - 498.835,
    diff_ratio = 3.389,
    fmt = '-g',
    max_rpm=4500)

plt.xlabel('km/h')
plt.ylabel('thrust N')
#plt.legend()
plt.ylim(0, 15000)
plt.xlim(0, max_kmph)
plt.grid()
plt.show()