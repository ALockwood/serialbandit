   M S B A M L   `   `   `  ����   SerialBandit    SerialBandit.WindowMain   \ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    8x,http://schemas.microsoft.com/winfx/2006/xaml    -     � Title$1  +SerialBandit - 5xx Series Roomba Controller��$	��365��$	��666�� � Icon$% RoombaBlack_Icon_Trans16x16.png�� � 
ResizeMode$ NoResize=�.��� .+��� -   $��sb_bottomStatus��$��23��$��Bottom=�
�������$��1q�.@��� -   $��sbi_portName��$��0q�
�������
�������$��Center=�$��Center=��� �� -   $��sbi_baudRate��$��0q�
�������
�������$��Center=�$��Center=��� �� -   $��sbi_CurrentTime��$��0q�
�������
�������$��Right=�$��Right=�$��Center=�$��italic$��� -   $��sbi_CurrentTimeLbl��$��0q�
�������
�������$��Right=�$��Right=�$��Center=�$��italic$�.��Current Time:o� -   $��menu1��$��22��$	��Top=�.@�m� -   $��	mnui_File��$
��File��.@�m� -	   $��mnui_File_quit��$
��Quit��m� -
   $��mnui_Settings��$��Settings��.@�m� -   $��mnui_Settings_com��$��COM Port Settings��m� -   $��mnui_Settings_rmbamodel��$��Roomba Model��.@�m� -   $��rmbamodel_5xx��$	��5xx�� m� 	IsChecked . m� -   $��menu_saveCleanSched��$$��Save Cleaning Schedule On Exit�� m� IsCheckable .  . m� -   $��mnui_Diagnostics��$��Diagnostics��.@�m� -   $��mnui_Diagnostics_testcom��$��Communications Test��.@�m� -   $#��mnui_Diagnostics_testcom_beep��$
��Beep��m� -   $$��mnui_Diagnostics_testcom_blink��$��Blink Light��m� -   $��mnui_Diagnostics_passive��$ ��Put Roomba in Passive Mode��m� -   $��mnui_Diagnostics_safe��$��Put Roomba into Safe Mode��m� -   $��mnui_Diagnostics_direct��$��Put Roomba in Direct Mode��m� -   $��mnui_Diagnostics_reset��$��Reset Roomba��m� -   $��mnui_CleaningCmds��$��Cleaning Commands��.@�m� -   $ ��mnui_CleaningCmds_seekdock��$��	Seek Dock��m� -   $��mnui_CleaningCmds_clean��$��Start Cleaning Cycle��m� -   $ ��mnui_CleaningCmds_maxclean��$��Start Max Cleaning Cycle��m� -   $!��mnui_CleaningCmds_spotclean��$��
Spot Clean��m� -   $��	mnui_Help��$
��Help��.@�m� -   $��mnui_Help_about��$��About...���� -   $��chk_mon��$��16��$��
18,0,0,222q�$��Bottom=�$
��Left=�$��72��}�.  .��Monday�� -   $��chk_tue��$��16��$��
18,0,0,197q�$��Bottom=�$
��Left=�$��72��}�.  .��	Tuesday�� -    $��chk_wed��$��16��$��
18,0,0,172q�$��Bottom=�$
��Left=�$��78��}�.  .��	Wednesday�� -!   $��chk_thu��$��
18,0,0,148q�$
��Left=�$��72��}�.  $��15��$��Bottom=�.��
Thursday�� -"   $��chk_fri��$��16��$��
18,0,0,122q�$��Bottom=�$
��Left=�$��72��}�.  .��Friday�� -#   $��chk_sat��$��16��$��	18,0,0,97q�$��Bottom=�$
��Left=�$��72��}�.  .��
Saturday�� -$   $��chk_sun��$��16��$��	18,0,0,72q�$��Bottom=�$
��Left=�$��72��}�.  .��Sunday�� -%   $��sl_mon��$��24��$��100,0,71,216q�$��Bottom=� �� Delay$ 0�� � Maximum$
 1439Q� � SmallChange$ 1Q� �� Interval$ 1��	 �� TickPlacement$
	 None=�
 �� IsSnapToTickEnabled
 .  �� IsMoveToPointEnabled .  � ToolTip$0 *Use the arrow keys to fine-tune selections��{�.  }�.  �� -&   $��sl_tue��$��24��$��100,0,71,191q�$��Bottom=�$ 0��$
 1439Q�$ 1Q�$ 1��$
	 None=�
 .  . $0 *Use the arrow keys to fine-tune selections��{�.  }�.  �� -'   $��sl_wed��$��24��$��100,0,71,166q�$��Bottom=�$ 0��$
 1439Q�$ 1Q�$ 1��$
	 None=�
 .  . $0 *Use the arrow keys to fine-tune selections��{�.  }�.  �� -(   $��sl_thu��$��100,0,71,141q�$ 0��$
 1439Q�$ 1Q�$ 1��$
	 None=�
 .  . $0 *Use the arrow keys to fine-tune selections��{�.  $��24��}�.  $��Bottom=��� -)   $��sl_fri��$��24��$��100,0,71,116q�$��Bottom=�$ 0��$
 1439Q�$ 1Q�$ 1��$
	 None=�
 .  . $0 *Use the arrow keys to fine-tune selections��{�.  }�.  �� -*   $��sl_sat��$��24��$��100,0,71,91q�$��Bottom=�$ 0��$
 1439Q�$ 1Q�$ 1��$
	 None=�
 .  . $0 *Use the arrow keys to fine-tune selections��{�.  }�.  �� -+   $��sl_sun��$��24��$��100,0,71,66q�$��Bottom=�$ 0��$
 1439Q�$ 1Q�$ 1��$
	 None=�
 .  . $0 *Use the arrow keys to fine-tune selections��{�.  }�.  �� -,   $��txt_mon��$��24��$��
0,0,12,216q�$��Bottom=�$�� ��$	��0pxq�$��Right=�$��58��$��Center=���.   J� 	AllowDrop .   �� IsUndoEnabled .   �� 
IsReadOnly . }�.  �� --   $��txt_tue��$��24��$��
0,0,12,191q�$��Bottom=�$�� ��$	��0pxq�$��Right=�$��58��$��Center=���.   .   .   . }�.  �� -.   $��txt_wed��$��24��$��
0,0,12,166q�$��Bottom=�$�� ��$	��0pxq�$��Right=�$��58��$��Center=���.   .   .   . }�.  �� -/   $��txt_thu��$��24��$��
0,0,12,141q�$��Bottom=�$�� ��$	��0pxq�$��Right=�$��58��$��Center=���.   .   .   . }�.  �� -0   $��txt_fri��$��24��$��
0,0,12,116q�$��Bottom=�$�� ��$	��0pxq�$��Right=�$��58��$��Center=���.   .   .   . }�.  �� -1   $��txt_sat��$��24��$��	0,0,12,91q�$��Bottom=�$�� ��$	��0pxq�$��Right=�$��58��$��Center=���.   .   .   . }�.  �� -2   $��txt_sun��$��	0,0,12,66q�$�� ��$	��0pxq�$��Right=�$��58��$��Center=���.   .   .   . }�.  $��24��$��Bottom=��� -3   $��
textBlock1��$��30��$��
85,46,53,0q�$	��Top=�$
��Wrap=� �� TextAlignment$
 Left=�$��light"�$��italic$�}�.  zxCheck the box to enable cleaning for the corresponding day. Then drag the slider to the time you want cleaning to start.�� -4   $��btn_SetSchedule��$��
0,0,224,37q�$��23��$��Bottom=�$��Right=�$��75��}�.  .��Set Schedule�� -5   $��btn_ResetSchedule��$��
0,0,118,37q�$��23��$��Right=�$��Bottom=�$��75��}�.  .��Reset Values