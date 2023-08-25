import win32process
import win32gui

def get_hwnds_for_pid (pid):
  def callback (hwnd, hwnds):
    if win32gui.IsWindowVisible (hwnd) and win32gui.IsWindowEnabled (hwnd):
      _, found_pid = win32process.GetWindowThreadProcessId (hwnd)
      if found_pid == pid:
        hwnds.append (hwnd)
    return True
    
  hwnds = []
  win32gui.EnumWindows (callback, hwnds)
  return hwnds

def get_window_rect(pid):
  hwnds = get_hwnds_for_pid (pid)
  if len(hwnds) == 1:
    return win32gui.GetWindowRect(hwnds[0])
  elif len(hwnds) == 0:
    print("No hwnds found for pid ", pid)
  else:
    print("Multiple hwnds for pid ",pid,":", hwnds)
  return None