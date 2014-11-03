<?php
//获取操作指令页面
//读取某个用户名对应的所有指令后则删除
if(!isset($_POST['username']))
{
	$error_msg = array('result' => '-1','message' => '请求所需信息不足，获取失败！');
	    echo json_encode($error_msg);	
		}
		//赋值
		$username = $_POST['username'];
		//打开数据库连接
		$link = @mysql_connect(SAE_MYSQL_HOST_M.':'.SAE_MYSQL_PORT,SAE_MYSQL_USER,SAE_MYSQL_PASS);
		if(!$link) 
		{
		    $error_msg = array();
			    $error_msg['result']='-1';
				    $error_msg['message']="连接失败！原因： " . mysql_error();
					    echo json_encode($error_msg);
						}
						//设置字符编码
						mysql_query("set names utf8",$link);
						//选择数据库
						if(!mysql_select_db(SAE_MYSQL_DB,$link)) 
						{
						    $error_msg = array();
							    $error_msg['result']='-1';
								    $error_msg['message']="选择数据库失败！原因：" . mysql_error($link);
									   	echo json_encode($error_msg);
										}

										$delete = "delete from Instructor where username = '$username'";
										$sql = "select content from Instructor where username='$username'";
										$arr = array();
										$index = 0;
										if($res = mysql_query($sql,$link))
										{
											while($row = mysql_fetch_assoc($res))
												{	
														$arr[$index++] = $row['content'];
															}
															    //删除记录
																    if(!($del = mysql_query($delete,$link)))
																	    {
																		        $error_msg = array('result' => '-1','message' => '删除记录失败！');
																				    	echo json_encode($error_msg);
																						        exit();
																								    }
																										//最后制作成json字符串返回便于读取
																											echo json_encode($arr);
																											}
																											else
																											{
																											    $error_msg = array('result' => '-1','message' => '获取失败！');
																												    echo json_encode($error_msg);
																													    exit();
																														}
																														?>
