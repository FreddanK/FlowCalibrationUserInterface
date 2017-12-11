clc
close all
clear all

%Select test
test = 4;
%Select subtest
subtest = 5;
%Select index
index = [1];
T = {'Flow profile: Sine, 1 rad/s, Peak flow 2 ml/s';...
    'Flow profile: Sine, 3 rad/s, Peak flow 20 ml/s';...
    'Flow profile: Sine, 6 rad/s, Peak flow 200 ml/s';...
    'Flow profile: Triangle, 3 rad/s, Peak flow 20 ml/s';...
    'Flow profile: Square, 3 rad/s, Peak flow 20 ml/s'};

for i = 1:length(index)
motor_vol = csvread(sprintf('%s.%s/Run%sVolume.csv',num2str(test),num2str(subtest),num2str(i)));
motor_time = motor_vol(:,1);
motor_pos = motor_vol(:,2);

x = csvread(sprintf('%s.%s/FlowProfile.csv',num2str(test),num2str(subtest)));
fp_time = x(:,1);
fp_f = x(:,2);

%[S(i),M(i)] = process_data(frame, pos, motor_vol(:,1),motor_vol(:,2),i);
[S(i),M(i)] = process_data2(fp_time, fp_f, motor_time,motor_pos,i);
figure(i)
title(T(index))
end

set(groot,'DefaultTextInterpreter','latex')
set(groot,'DefaultLegendInterpreter','latex')

