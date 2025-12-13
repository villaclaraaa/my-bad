export interface Hero {
  id: number;
  localized_name: string;
  primary_attr: 'str' | 'agi' | 'int' | 'all'; //all is for universal heroes - ODota name
  img: string; 
  icon: string;
}