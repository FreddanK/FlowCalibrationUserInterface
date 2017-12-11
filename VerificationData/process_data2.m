function [S,M] = process_data2(cam_times, cam_flow, motor_times, motor_vol, i)

if(size(cam_times,1)==1)
    cam_times = cam_times';
end
if(size(cam_flow,1)==1)
    cam_flow = cam_flow';
end
if(size(motor_times,1)==1)
    motor_times = motor_times';
end
if(size(motor_vol,1)==1)
    motor_vol = motor_vol';
end
cam_vol = integrate(cam_times,cam_flow);

S=0;
M=0;
% 
% 
% motor_flow = returnFlow(motor_vol,motor_times);
% motor_flow = [motor_flow(2:end);0];
% 
% f_vec = zeros(size(motor_flow));
% 
% for i = 1:length(motor_flow)
%    index = find( cam_times <= motor_times(i));
%    index = index(end);
%    f_vec(i)= cam_flow(index);
% end
% 
% E = f_vec - motor_flow;
% M = mean(E);
% S = std(E);
% 
% T = 0.00001; % Resolution 0.1 ms
% tf = 1000000; %Large time (infinate)
% m_t = 0:T:ceil(max(motor_times(end),cam_times(end))/T)*T; %New time vector
% M_V = interp1([motor_times; tf],[motor_flow; motor_flow(end)],m_t); 
% C_V = interp1([cam_times; tf],[cam_flow; cam_flow(end)],m_t);
% 
% M_V(isnan(M_V))=0;
% C_V(isnan(C_V))=0;
% 
% DT = finddelay(C_V,M_V)*T;
% cam_times_D = cam_times + DT;
% 
% m_flow = interp1(motor_times,motor_flow,cam_times_D);
% 
% % c = 1;
% % while(isnan(m_flow(c)) == 1)
% %     m_flow(c) = cam_vol(1);
% %     c = c+1;
% % end
% % c=length(C_Vol);
% % while(isnan(C_Vol(c)) == 1)
% %    C_Vol(c) = cam_vol(end);
% %    c = c-1; 
% % end
% 
% m_flow(isnan(m_flow))=0;
% motor_flow = [0;motor_flow(1:end-1)];
% 
figure(i);
plot(cam_times,cam_vol,'b'); hold on
%plot(motor_times,motor_vol,'--r')
plot(motor_times,motor_vol,'*r')
xlabel('Time [s]')
ylabel('Volume [ml]')
legend('Profile','System Measurements')
grid on;
end