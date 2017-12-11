clc
close all
clear all

%Select test
test = 1;
%Select subtest
subtest = 3;
%Select index
index = [1 2 3];

for i = 1:length(index)
load(sprintf('test%s.mat',num2str(test*100+subtest*10+index(i))));
motor_vol = csvread(sprintf('%s.%s/Run%sVolume.csv',num2str(test),num2str(subtest),num2str(i)));
motor_time = motor_vol(:,1);
motor_pos = motor_vol(:,2);
frame = eval([genvarname(sprintf('test_%s_frame',num2str(test*100+subtest*10+index(i))))]);
pos = eval([genvarname(sprintf('test_%s_pos',num2str(test*100+subtest*10+index(i))))]);
[S(i),M(i)] = process_data(frame, pos, motor_vol(:,1),motor_vol(:,2),i);
end
STD = mean(S)
MEAN = mean(M)