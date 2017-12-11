clc
close all
clear all

test_131_pos = [55 60 65 70 75 80 85 90 95 100 105 110 115 120 125 130 130 125 120 115 110 105 100 95 90 85 80 75 70 65 60]
test_131_frame = [146 163 172 179 185 190.5 196 201.5 207 213 219 224.5 231 239 248 268 274 290.5 299.5 306 312 317.5 323.5 329 334.5 340 346 352 359 368.5 390]
motor_vol_131 = csvread('Run1Volume.csv')
motor_131_time = motor_vol_131(:,1)
motor_131_pos = motor_vol_131(:,2)
[S131,M131] = process_data(test_131_frame, test_131_pos, motor_vol_131(:,1),motor_vol_131(:,2));
