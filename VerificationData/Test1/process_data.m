function [S,M] = process_data(cam_frames,cam_pos, motor_times, motor_vol, i)

if(size(cam_frames,1)==1)
    cam_frames = cam_frames';
end
if(size(cam_pos,1)==1)
    cam_pos = cam_pos';
end
if(size(motor_times,1)==1)
    motor_times = motor_times';
end
if(size(motor_vol,1)==1)
    motor_vol = motor_vol';
end

A = 1.7^2*pi;
cam_times = (cam_frames-cam_frames(1))/240;
cam_pos = cam_pos-cam_pos(1);
cam_vol = cam_pos/10*A;
motor_times = motor_times-motor_times(1);

T = 0.0001; % Resolution 0.1 ms
tf = 1000000; %Large time (infinate)
m_t = 0:T:ceil(max(motor_times(end),cam_times(end))/T)*T; %New time vector
M_V = interp1([motor_times; tf],[motor_vol; motor_vol(end)],m_t); 
C_V = interp1([cam_times; tf],[cam_vol; cam_vol(end)],m_t);
DT = finddelay(C_V,M_V)*T;
cam_times = cam_times + DT;

% C_Vol = interp1(cam_times,cam_vol,motor_times);
% C_Vol(isnan(C_Vol(1:sum(isnan(C_Vol))))) = cam_vol(1);
% C_Vol(isnan(C_Vol(end+1-sum(isnan(C_Vol)):end))) = cam_vol(end);
%  c=1;
%  while(isnan(C_Vol(c))==1)
%      C_Vol(c) = cam_vol(1);
%      c=c+1;
%  end
%  c=length(C_Vol);
%  while(isnan(C_Vol(c))==1)
%      C_Vol(c) = cam_vol(end);
%      c=c-1;
%  end


M_Vol = interp1(motor_times,motor_vol,cam_times);
c=1;
while(isnan(M_Vol(c))==1)
    M_Vol(c) = motor_vol(1);
    c=c+1;
end
c=length(M_Vol);
while(isnan(M_Vol(c))==1)
    M_Vol(c) = motor_vol(end);
    c=c-1;
end

%E = motor_vol - C_Vol;
E = M_Vol - cam_vol;
M = mean(E);
S = std(E);

figure(i);
plot(cam_times,cam_vol); hold on
plot(motor_times, motor_vol)
end